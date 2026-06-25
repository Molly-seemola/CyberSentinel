using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Speech.Synthesis;
using CyberSentinel.Models;

namespace CyberSentinel
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _db = new DatabaseHelper();
        private QuizManager _quiz = new QuizManager();
        private bool _quizInProgress = false;
        private int _selectedOptionIndex = -1;

        // Voice synthesis
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();
        private bool _voiceEnabled = true;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            // Configure voice (optional)
            try
            {
                var voices = _speechSynthesizer.GetInstalledVoices();
                foreach (var voice in voices)
                {
                    if (voice.VoiceInfo.Gender == VoiceGender.Female)
                    {
                        _speechSynthesizer.SelectVoice(voice.VoiceInfo.Name);
                        break;
                    }
                }
                _speechSynthesizer.Rate = 0;
                _speechSynthesizer.Volume = 100;
            }
            catch { } // fallback to default
        }

        // ============================================================
        // Window Load – Welcome message, sound, load tasks
        // ============================================================
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SoundManager.PlayWelcome();
            string welcome = "Welcome! I'm your cybersecurity assistant. How can I help you today?";
            AppendChatMessage(" Cyber Sentinel", welcome);
            SpeakResponse(welcome);
            ActivityLogger.Log("Application started");
            RefreshTasks();
            RefreshLog();
        }

        // ============================================================
        // Voice / Sound Toggles
        // ============================================================
        private void BtnVoiceToggle_Click(object sender, RoutedEventArgs e)
        {
            _voiceEnabled = !_voiceEnabled;
            btnVoiceToggle.Content = _voiceEnabled ? "🔊 Voice On" : "🔇 Voice Off";
            btnVoiceToggle.Background = _voiceEnabled ?
                new SolidColorBrush(Color.FromRgb(41, 128, 185)) :
                new SolidColorBrush(Color.FromRgb(189, 195, 199));
        }

        private void BtnSoundToggle_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.SoundEnabled = !SoundManager.SoundEnabled;
            btnSoundToggle.Content = SoundManager.SoundEnabled ? "🔊 Sound On" : "🔇 Sound Off";
            btnSoundToggle.Background = SoundManager.SoundEnabled ?
                new SolidColorBrush(Color.FromRgb(39, 174, 96)) :
                new SolidColorBrush(Color.FromRgb(189, 195, 199));
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cyber Sentinel Help\n\n" +
                " Chat: Type your message. I understand tasks, reminders, quiz, and logs.\n" +
                " Tasks: Add, complete, delete tasks with reminders.\n" +
                " Quiz: Test your cybersecurity knowledge.\n" +
                " Activity Log: View recent actions.\n\n" +
                "Examples:\n" +
                "  - 'Add task: Enable 2FA'\n" +
                "  - 'Remind me to update password tomorrow'\n" +
                "  - 'Start quiz'\n" +
                "  - 'Show activity log'",
                "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ============================================================
        // Chat Input Handling
        // ============================================================
        private void TxtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnSend_Click(sender, e);
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            string userInput = txtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;

            AppendChatMessage("You", userInput);
            txtUserInput.Clear();
            SoundManager.PlayResponse();

            // Process with NLP
            var (intent, entity, reminderDate) = NLPSimulator.ParseInput(userInput);

            string response = "";

            switch (intent)
            {
                case "add_task":
                    if (string.IsNullOrEmpty(entity)) entity = "Cybersecurity task";
                    int taskId = _db.AddTask(entity, entity, null);
                    if (taskId > 0)
                    {
                        response = $"✅ Task added: \"{entity}\". Would you like a reminder? (say 'set reminder')";
                        ActivityLogger.Log($"Task added: {entity}");
                    }
                    else
                        response = "❌ Failed to add task. Please try again.";
                    break;

                case "set_reminder":
                    if (!string.IsNullOrEmpty(entity))
                    {
                        var tasks = _db.GetTasks();
                        TaskItem target = null;
                        foreach (var t in tasks)
                        {
                            if (!t.IsCompleted && t.Title.ToLower().Contains(entity.ToLower()))
                            {
                                target = t;
                                break;
                            }
                        }
                        if (target != null)
                        {
                            DateTime reminder = DateTime.Today;
                            if (!string.IsNullOrEmpty(reminderDate) && DateTime.TryParse(reminderDate, out DateTime parsed))
                                reminder = parsed;
                            else
                                reminder = DateTime.Today.AddDays(1);

                            _db.UpdateTask(target.Id, target.Title, target.Description, reminder);
                            response = $"⏰ Reminder set for \"{target.Title}\" on {reminder:yyyy-MM-dd}.";
                            ActivityLogger.Log($"Reminder set for task: {target.Title}");
                        }
                        else
                            response = "❌ I couldn't find a matching task to set a reminder for.";
                    }
                    else
                        response = "Please specify what you want to be reminded about.";
                    break;

                case "start_quiz":
                    _quiz.Reset();
                    _quizInProgress = true;
                    tabControl.SelectedItem = tabQuiz;
                    LoadNextQuizQuestion();
                    response = " Quiz started! Good luck!";
                    ActivityLogger.Log("Quiz started");
                    break;

                case "show_log":
                    var logs = ActivityLogger.GetRecentLogs(10);
                    response = " Recent activity:\n" + string.Join("\n", logs);
                    break;

                case "chat":
                default:
                    if (userInput.ToLower().Contains("hello") || userInput.ToLower().Contains("hi"))
                        response = "Hello! How can I assist you with your cybersecurity today?";
                    else if (userInput.ToLower().Contains("password"))
                        response = "Remember: Use strong passwords and enable two-factor authentication!";
                    else if (userInput.ToLower().Contains("phishing"))
                        response = "Phishing attacks are common. Always verify email senders and avoid clicking suspicious links.";
                    else if (userInput.ToLower().Contains("thanks") || userInput.ToLower().Contains("thank"))
                        response = "You're welcome! Stay safe online!";
                    else
                        response = "I'm here to help with cybersecurity tasks, reminders, quizzes, and more. Try saying 'add task', 'start quiz', or 'show activity log'.";
                    break;
            }

            AppendChatMessage("Cyber Sentinel", response);
            SpeakResponse(response); // Voice output
            ActivityLogger.Log($"Response given to: {userInput}");
            RefreshLog();
        }

        // ============================================================
        // Helper: Append to Chat
        // ============================================================
        private void AppendChatMessage(string sender, string message)
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(sender + ": ") { FontWeight = FontWeights.Bold, Foreground = Brushes.DarkBlue });
            paragraph.Inlines.Add(new Run(message ?? string.Empty));
            rtbChatHistory.Document.Blocks.Add(paragraph);
            chatScrollViewer.ScrollToEnd();
        }

        // ============================================================
        // Helper: Speak Response (Text-to-Speech)
        // ============================================================
        private void SpeakResponse(string text)
        {
            if (!_voiceEnabled || string.IsNullOrWhiteSpace(text)) return;
            try
            {
                _speechSynthesizer.SpeakAsync(text);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TTS Error: {ex.Message}");
            }
        }

        // ============================================================
        // Task Management
        // ============================================================
        private void RefreshTasks()
        {
            var tasks = _db.GetTasks();
            dgTasks.ItemsSource = tasks;
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            string desc = txtTaskDesc.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a task title.", "Missing Title", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime? reminder = null;
            if (chkReminder.IsChecked == true && dpReminder.SelectedDate.HasValue)
                reminder = dpReminder.SelectedDate.Value;

            int id = _db.AddTask(title, desc, reminder);
            if (id > 0)
            {
                MessageBox.Show($"Task '{title}' added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ActivityLogger.Log($"Task added via GUI: {title}");
                RefreshTasks();
                txtTaskTitle.Clear();
                txtTaskDesc.Clear();
                chkReminder.IsChecked = false;
                dpReminder.SelectedDate = null;
            }
            else
                MessageBox.Show("Failed to add task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ChkReminder_Checked(object sender, RoutedEventArgs e) => dpReminder.IsEnabled = true;
        private void ChkReminder_Unchecked(object sender, RoutedEventArgs e) { dpReminder.IsEnabled = false; dpReminder.SelectedDate = null; }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTasks.SelectedItem is TaskItem selected)
            {
                if (_db.MarkComplete(selected.Id))
                {
                    ActivityLogger.Log($"Task completed: {selected.Title}");
                    RefreshTasks();
                    MessageBox.Show("Task marked as completed.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("Failed to update task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Please select a task.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTasks.SelectedItem is TaskItem selected)
            {
                if (MessageBox.Show($"Delete task '{selected.Title}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (_db.DeleteTask(selected.Id))
                    {
                        ActivityLogger.Log($"Task deleted: {selected.Title}");
                        RefreshTasks();
                    }
                    else
                        MessageBox.Show("Failed to delete task.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("Please select a task.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnRefreshTasks_Click(object sender, RoutedEventArgs e) => RefreshTasks();

        // ============================================================
        // Quiz Logic
        // ============================================================
        private void BtnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quiz.Reset();
            _quizInProgress = true;
            _selectedOptionIndex = -1;
            btnStartQuiz.IsEnabled = false;
            btnSubmitAnswer.IsEnabled = false;
            LoadNextQuizQuestion();
            ActivityLogger.Log("Quiz started via GUI");
            AppendChatMessage("Cyber Sentinel", " Quiz started! Answer each question to test your cybersecurity knowledge.");
        }

        private void LoadNextQuizQuestion()
        {
            var q = _quiz.GetCurrentQuestion();
            if (q == null || _quiz.IsFinished())
            {
                string msg = _quiz.GetFinalMessage();
                txtQuestion.Text = " Quiz Complete!";
                icOptions.ItemsSource = null;
                txtQuizFeedback.Text = msg + "\n\nClick 'Start New Quiz' to try again.";
                txtQuizFeedback.Foreground = Brushes.Black;
                btnSubmitAnswer.IsEnabled = false;
                btnStartQuiz.IsEnabled = true;
                _quizInProgress = false;
                txtQuizScore.Text = $"Final Score: {_quiz.Score}/{_quiz.TotalQuestions}";
                txtQuizProgress.Text = "Complete!";
                ActivityLogger.Log($"Quiz completed. Score: {_quiz.Score}/{_quiz.TotalQuestions}");
                return;
            }

            txtQuestion.Text = q.Text;
            icOptions.ItemsSource = q.Options;
            txtQuizFeedback.Text = "";
            txtQuizFeedback.Foreground = Brushes.Black;
            btnSubmitAnswer.IsEnabled = false;
            _selectedOptionIndex = -1;

            txtQuizScore.Text = $"Score: {_quiz.Score}";
            txtQuizProgress.Text = $"Question {_quiz.CurrentIndex + 1}/{_quiz.TotalQuestions}";
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                var parent = FindParent<ItemsControl>(rb);
                if (parent != null)
                {
                    _selectedOptionIndex = parent.Items.IndexOf(rb.DataContext);
                    btnSubmitAnswer.IsEnabled = true;
                }
            }
        }

        private T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as T;
        }

        private void BtnSubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!_quizInProgress)
            {
                MessageBox.Show("Please start a new quiz first.", "No Quiz Active", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_selectedOptionIndex < 0)
            {
                MessageBox.Show("Please select an answer first.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var q = _quiz.GetCurrentQuestion();
            if (q == null)
            {
                MessageBox.Show("No question available. Please start a new quiz.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string feedback = _quiz.SubmitAnswer(_selectedOptionIndex, out bool correct, out int newScore);
            txtQuizScore.Text = $"Score: {newScore}";
            txtQuizFeedback.Text = feedback;
            txtQuizFeedback.Foreground = correct ? Brushes.Green : Brushes.Red;

            if (correct)
                SoundManager.PlayCorrect();
            else
                SoundManager.PlayIncorrect();

            ActivityLogger.Log($"Quiz answer: {(correct ? "Correct" : "Incorrect")} - {q.Text}");

            btnSubmitAnswer.IsEnabled = false;
            _selectedOptionIndex = -1;

            // Load next question after a short delay
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.5);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                if (_quizInProgress) LoadNextQuizQuestion();
            };
            timer.Start();
        }

        // ============================================================
        // Activity Log Refresh
        // ============================================================
        private void RefreshLog()
        {
            var logs = ActivityLogger.GetRecentLogs(10);
            lbActivityLog.ItemsSource = logs;
        }

        private void BtnRefreshLog_Click(object sender, RoutedEventArgs e) => RefreshLog();

        private void BtnShowAllLog_Click(object sender, RoutedEventArgs e)
        {
            var logs = ActivityLogger.GetAllLogs();
            lbActivityLog.ItemsSource = logs;
        }
    }
}