using System.Collections.Generic;

namespace CyberSentinel
{
    /// <summary>
    /// Manages the cybersecurity quiz.
    /// </summary>
    public class QuizManager
    {
        public class Question
        {
            public string Text { get; set; }
            public List<string> Options { get; set; }   // For multiple choice; for True/False, use two options.
            public int CorrectIndex { get; set; }
            public string Explanation { get; set; }
            public bool IsTrueFalse { get; set; } // If true, options are ["True","False"]
        }

        private List<Question> _questions;
        private int _currentIndex = 0;
        private int _score = 0;

        public QuizManager()
        {
            LoadQuestions();
        }

        private void LoadQuestions()
        {
            _questions = new()
            {
                new Question
                {
                    Text = "What should you do if you receive an email asking for your password?",
                    Options = new() { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams."
                },
                new Question
                {
                    Text = "Which of the following is a strong password?",
                    Options = new() { "password123", "P@ssw0rd!", "qwerty", "123456" },
                    CorrectIndex = 1,
                    Explanation = "A strong password includes uppercase, lowercase, numbers, and special characters."
                },
                new Question
                {
                    Text = "True or False: Using the same password for multiple accounts is safe.",
                    Options = new() { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Reusing passwords increases risk; if one account is compromised, others are vulnerable."
                },
                new Question
                {
                    Text = "What is phishing?",
                    Options = new() { "A type of fish", "A social engineering attack to steal credentials", "A secure email protocol", "An antivirus software" },
                    CorrectIndex = 1,
                    Explanation = "Phishing tricks users into giving sensitive information via fake communications."
                },
                new Question
                {
                    Text = "Which of these is a safe browsing practice?",
                    Options = new() { "Clicking on pop-up ads", "Using HTTPS websites", "Downloading from untrusted sources", "Sharing cookies" },
                    CorrectIndex = 1,
                    Explanation = "HTTPS encrypts data, making browsing safer."
                },
                new Question
                {
                    Text = "True or False: Two-factor authentication adds an extra layer of security.",
                    Options = new() { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "2FA requires a second verification step, reducing unauthorized access."
                },
                new Question
                {
                    Text = "What is social engineering?",
                    Options = new() { "A software bug", "Manipulating people to reveal confidential info", "A network attack", "An encryption method" },
                    CorrectIndex = 1,
                    Explanation = "Social engineering exploits human psychology rather than technical vulnerabilities."
                },
                new Question
                {
                    Text = "How often should you update your passwords?",
                    Options = new() { "Never", "Every year", "Every 3-6 months", "Only when forced" },
                    CorrectIndex = 2,
                    Explanation = "Regular updates reduce the risk of old passwords being compromised."
                },
                new Question
                {
                    Text = "True or False: Public Wi-Fi is always safe for banking transactions.",
                    Options = new() { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Public Wi-Fi may be unsecured; avoid sensitive transactions without a VPN."
                },
                new Question
                {
                    Text = "What is ransomware?",
                    Options = new() { "A type of firewall", "Malware that encrypts files and demands payment", "An antivirus tool", "A password manager" },
                    CorrectIndex = 1,
                    Explanation = "Ransomware locks files and asks for ransom to unlock them."
                },
                new Question
                {
                    Text = "Which of the following is a good practice for email security?",
                    Options = new() { "Open attachments from unknown senders", "Click on links in suspicious emails", "Verify sender's address before replying", "Forward chain emails" },
                    CorrectIndex = 2,
                    Explanation = "Always verify sender addresses to avoid phishing."
                }
            };
        }

        /// <summary>
        /// Returns the current question (or null if finished).
        /// </summary>
        public Question GetCurrentQuestion()
        {
            if (_currentIndex < _questions.Count)
                return _questions[_currentIndex];
            return null;
        }

        /// <summary>
        /// Submits an answer and returns a feedback string.
        /// </summary>
        public string SubmitAnswer(int selectedIndex, out bool correct, out int newScore)
        {
            var q = GetCurrentQuestion();
            if (q == null) { correct = false; newScore = _score; return "No question."; }

            correct = (selectedIndex == q.CorrectIndex);
            if (correct) _score++;

            // Move to next question
            _currentIndex++;

            newScore = _score;
            string feedback = correct ? "✅ Correct! " : "❌ Incorrect. ";
            feedback += q.Explanation;

            return feedback;
        }

        /// <summary>
        /// Returns true if all questions have been answered.
        /// </summary>
        public bool IsFinished() => _currentIndex >= _questions.Count;

        /// <summary>
        /// Resets the quiz.
        /// </summary>
        public void Reset()
        {
            _currentIndex = 0;
            _score = 0;
        }

        /// <summary>
        /// Gets the final score message.
        /// </summary>
        public string GetFinalMessage()
        {
            int total = _questions.Count;
            double percent = (double)_score / total * 100;
            if (percent >= 80) return $"Great job! You're a cybersecurity pro! ({_score}/{total})";
            else if (percent >= 50) return $"Good effort! Keep learning to stay safe online! ({_score}/{total})";
            else return $"Keep learning to stay safe online! ({_score}/{total})";
        }

        public int TotalQuestions => _questions.Count;
        public int CurrentIndex => _currentIndex;
        public int Score => _score;
    }
}