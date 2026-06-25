 Cyber Sentinel – Cybersecurity Awareness Chatbot
A fully featured WPF desktop application that helps users manage cybersecurity tasks, test their knowledge with an interactive quiz, and interact with a voice‑enabled assistant. Built with C#, XAML, MySQL, and System.Speech.

 Features
Chat Interface – Natural language processing simulation understands tasks, reminders, quiz, and logs.

Task Assistant – Create, complete, delete tasks with optional reminders; all stored in MySQL.

Cybersecurity Quiz – 11 questions with multiple‑choice and True/False; immediate feedback and scoring.

Activity Log – Tracks all actions (tasks, reminders, quiz, NLP commands) with timestamps.

Sound Feedback – Plays welcome, response, and quiz sounds via SoundManager.

Voice (Text‑to‑Speech) – Reads chatbot responses aloud using Windows TTS (toggle on/off).

Prerequisites
Windows 10/11 (for Speech synthesis and sound playback)

Visual Studio 2019 or later (with .NET desktop development workload)

MySQL Server 8.0 (or compatible) running locally

MySQL Workbench (optional, for database management)

 Required NuGet Packages & References
Package / Reference	How to install
MySql.Data	NuGet: Install-Package MySql.Data
System.Speech	Add Reference → Assemblies → Framework → check System.Speech Database Setup
Open MySQL (command line or Workbench) and run:

sql
CREATE DATABASE cybersentinel;
USE cybersentinel;

CREATE TABLE tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    reminder_date DATETIME NULL,
    is_completed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
Verify the table exists:

sql
SHOW TABLES;
DESCRIBE tasks;
Configuration
Open DatabaseHelper.cs and update the connection string with your MySQL credentials:

csharp
private const string ConnectionString = "Server=localhost;Database=cybersentinel;Uid=root;Pwd=Molly@006;";
How to Run
Clone or download the source code.


 Usage Guide Chat Tab
Type a message and press Enter or click Send.

The chatbot understands:

add task: ... – creates a task (e.g., "Add task: Enable 2FA")

remind me to ... [tomorrow/in 3 days] – sets a reminder on an existing task

start quiz – begins the cybersecurity quiz

show activity log – displays recent actions

📋 Tasks Tab
Add a task with a title, description, and optional reminder date.

Mark Complete – updates the task status in the database.

Delete – permanently removes the task.

The task list automatically refreshes after every action.

🧠 Quiz Tab
Click Start New Quiz to begin.

Select an answer and click Submit Answer.

You receive immediate feedback with an explanation and sound effect.

After all questions, your final score is displayed with a motivational message.

📜 Activity Log Tab
View the last 10 actions; click Show All to see the full history.

The log captures all task operations, quiz activity, NLP commands, and more.

🔊 Voice & Sounds
Use the top‑right buttons to toggle Voice (text‑to‑speech) and Sound (beeps) independently.

📝 Example Interactions
User Input	Chatbot Response
Hello	Hello! How can I assist you with your cybersecurity today?
Add task: Review privacy settings	✅ Task added: "Review privacy settings". Would you like a reminder? (say 'set reminder')
Remind me to review privacy settings tomorrow	⏰ Reminder set for "Review privacy settings" on 2026-06-26.
Start quiz	🧠 Quiz started! Good luck!
Show activity log	📜 Recent activity:\n2026-06-25 10:15:30 - Task added: Review privacy settings\n...
🌐 Video Presentation
Click here to watch the demo and code walkthrough


🙏 Acknowledgments
Built as a final project for the Cybersecurity Awareness Chatbot assignment.

Uses MySQL for data persistence and System.Speech for voice synthesis.

📞 Support
For issues, please open an issue on GitHub or contact [your email] – I'll be happy to help.

Stay safe online! 🛡️
