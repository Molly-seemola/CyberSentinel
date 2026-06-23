using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using CyberSentinel.Models;

namespace CyberSentinel
{
    /// <summary>
    /// Database helper for MySQL operations.
    /// </summary>
    public class DatabaseHelper
    {
        // Change this connection string to your MySQL setup
        private const string ConnectionString = "Server=localhost;Database=cybersentinel;Uid=root;Pwd=Molly@006;";

        /// <summary>
        /// Adds a new task to the database.
        /// </summary>
        public int AddTask(string title, string description, DateTime? reminderDate)
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            string query = @"INSERT INTO tasks (title, description, reminder_date) 
                             VALUES (@title, @desc, @reminder);
                             SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@desc", description ?? string.Empty);
            cmd.Parameters.AddWithValue("@reminder", reminderDate.HasValue ? (object)reminderDate.Value : DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Retrieves all tasks.
        /// </summary>
        public List<TaskItem> GetTasks()
        {
            var tasks = new List<TaskItem>();
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            string query = "SELECT id, title, description, reminder_date, is_completed, created_at FROM tasks";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    ReminderDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                    IsCompleted = reader.GetBoolean(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }
            return tasks;
        }

        /// <summary>
        /// Marks a task as completed.
        /// </summary>
        public bool MarkComplete(int taskId)
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", taskId);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Deletes a task.
        /// </summary>
        public bool DeleteTask(int taskId)
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            string query = "DELETE FROM tasks WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", taskId);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Updates task details (title, description, reminder).
        /// </summary>
        public bool UpdateTask(int taskId, string title, string description, DateTime? reminderDate)
        {
            using var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            string query = @"UPDATE tasks SET title = @title, description = @desc, 
                             reminder_date = @reminder WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@desc", description ?? string.Empty);
            cmd.Parameters.AddWithValue("@reminder", reminderDate.HasValue ? (object)reminderDate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@id", taskId);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}