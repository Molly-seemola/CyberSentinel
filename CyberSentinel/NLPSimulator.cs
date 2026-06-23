using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CyberSentinel
{
    /// <summary>
    /// Simulates basic NLP: extracts intent and entities from user input.
    /// </summary>
    public static class NLPSimulator
    {
        /// <summary>
        /// Analyzes user input and returns a structured intent.
        /// </summary>
        public static (string intent, string entity, string reminderDate) ParseInput(string input)
        {
            input = input.ToLower().Trim();

            // Detect task-related intents
            if (input.Contains("add task") || input.Contains("create task") || input.Contains("new task"))
            {
                // Extract task description: everything after "add task" etc.
                string taskDesc = ExtractAfterPhrase(input, new[] { "add task", "create task", "new task" });
                if (string.IsNullOrEmpty(taskDesc)) taskDesc = "cybersecurity task";
                return ("add_task", taskDesc, null);
            }

            // Detect reminder intents
            if (input.Contains("remind me") || input.Contains("set reminder") || input.Contains("add reminder"))
            {
                // Extract reminder description
                string reminderDesc = ExtractAfterPhrase(input, new[] { "remind me", "set reminder", "add reminder" });
                // Try to find date: tomorrow, in X days, etc.
                string date = ExtractDate(input);
                return ("set_reminder", reminderDesc ?? "reminder", date);
            }

            // Quiz intent
            if (input.Contains("quiz") || input.Contains("start quiz") || input.Contains("play quiz"))
            {
                return ("start_quiz", null, null);
            }

            // Activity log intent
            if (input.Contains("activity") || input.Contains("what have you done") || input.Contains("show log"))
            {
                return ("show_log", null, null);
            }

            // General chat / fallback
            return ("chat", input, null);
        }

        private static string ExtractAfterPhrase(string input, string[] phrases)
        {
            foreach (var phrase in phrases)
            {
                int idx = input.IndexOf(phrase);
                if (idx >= 0)
                {
                    string rest = input.Substring(idx + phrase.Length).Trim();
                    if (!string.IsNullOrEmpty(rest)) return rest;
                }
            }
            return null;
        }

        private static string ExtractDate(string input)
        {
            // Simple detection: "tomorrow", "in 3 days", "next week"
            if (input.Contains("tomorrow")) return DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
            var match = Regex.Match(input, @"in (\d+) days?");
            if (match.Success)
            {
                int days = int.Parse(match.Groups[1].Value);
                return DateTime.Today.AddDays(days).ToString("yyyy-MM-dd");
            }
            if (input.Contains("next week")) return DateTime.Today.AddDays(7).ToString("yyyy-MM-dd");
            return null; // default to no reminder
        }
    }
}