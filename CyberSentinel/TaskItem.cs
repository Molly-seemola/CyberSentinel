using System;

namespace CyberSentinel.Models
{
    /// <summary>
    /// Represents a cybersecurity task stored in the database.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}