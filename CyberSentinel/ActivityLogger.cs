using System;
using System.Collections.Generic;

namespace CyberSentinel
{
    /// <summary>
    /// Logs all significant actions performed by the chatbot.
    /// </summary>
    public static class ActivityLogger
    {
        private static List<string> _log = new();

        /// <summary>
        /// Adds an entry to the log.
        /// </summary>
        public static void Log(string action)
        {
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {action}";
            _log.Add(entry);
        }

        /// <summary>
        /// Returns the last N entries (default 10).
        /// </summary>
        public static List<string> GetRecentLogs(int count = 10)
        {
            if (_log.Count == 0) return new() { "No activity logged yet." };
            int start = Math.Max(0, _log.Count - count);
            return _log.GetRange(start, _log.Count - start);
        }

        /// <summary>
        /// Returns all logs.
        /// </summary>
        public static List<string> GetAllLogs() => new(_log);
    }
}
