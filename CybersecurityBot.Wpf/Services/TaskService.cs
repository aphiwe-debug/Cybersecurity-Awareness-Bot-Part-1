using CybersecurityBot.Wpf.Models;
using MySql.Data.MySqlClient;

namespace CybersecurityBot.Wpf.Services;

/// <summary>All task CRUD against MySQL. Uses parameterised queries.</summary>
public class TaskService
{
    private readonly DatabaseService _db;
    private readonly Action<string>? _log;

    public TaskService(DatabaseService db, Action<string>? log = null)
    {
        _db = db;
        _log = log;
    }

    public TaskItem AddTask(string title, string description, DateTime? reminder)
    {
        using var conn = _db.OpenConnection();
        using var cmd = new MySqlCommand(
            "INSERT INTO tasks (title, description, reminder_date) VALUES (@title, @desc, @reminder);", conn);
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@desc", description);
        cmd.Parameters.AddWithValue("@reminder", (object?)reminder ?? DBNull.Value);
        cmd.ExecuteNonQuery();

        var task = new TaskItem
        {
            Id = (int)cmd.LastInsertedId,
            Title = title,
            Description = description,
            ReminderDate = reminder,
            CreatedAt = DateTime.Now
        };
        _log?.Invoke(reminder.HasValue
            ? $"Task added: '{title}' (reminder set for {reminder:dd MMM yyyy HH:mm})"
            : $"Task added: '{title}' (no reminder set)");
        return task;
    }

    public List<TaskItem> GetAll()
    {
        var tasks = new List<TaskItem>();
        using var conn = _db.OpenConnection();
        using var cmd = new MySqlCommand(
            "SELECT id, title, description, reminder_date, is_completed, created_at FROM tasks " +
            "ORDER BY is_completed, reminder_date IS NULL, reminder_date, created_at DESC;", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int reminderOrdinal = reader.GetOrdinal("reminder_date");
            tasks.Add(new TaskItem
            {
                Id = reader.GetInt32("id"),
                Title = reader.GetString("title"),
                Description = reader.GetString("description"),
                ReminderDate = reader.IsDBNull(reminderOrdinal) ? null : reader.GetDateTime(reminderOrdinal),
                IsCompleted = reader.GetBoolean("is_completed"),
                CreatedAt = reader.GetDateTime("created_at")
            });
        }
        return tasks;
    }

    public void SetReminder(TaskItem task, DateTime reminder)
    {
        Execute("UPDATE tasks SET reminder_date = @r WHERE id = @id;", ("@r", reminder), ("@id", task.Id));
        task.ReminderDate = reminder;
        _log?.Invoke($"Reminder set: '{task.Title}' on {reminder:dd MMM yyyy HH:mm}");
    }

    public void MarkCompleted(TaskItem task)
    {
        Execute("UPDATE tasks SET is_completed = 1 WHERE id = @id;", ("@id", task.Id));
        task.IsCompleted = true;
        _log?.Invoke($"Task completed: '{task.Title}'");
    }

    public void Delete(TaskItem task)
    {
        Execute("DELETE FROM tasks WHERE id = @id;", ("@id", task.Id));
        _log?.Invoke($"Task deleted: '{task.Title}'");
    }

    private void Execute(string sql, params (string Name, object Value)[] args)
    {
        using var conn = _db.OpenConnection();
        using var cmd = new MySqlCommand(sql, conn);
        foreach (var (name, value) in args) cmd.Parameters.AddWithValue(name, value);
        cmd.ExecuteNonQuery();
    }
}