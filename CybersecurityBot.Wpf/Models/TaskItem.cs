namespace CybersecurityBot.Wpf.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? ReminderDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }

    public string ReminderText => ReminderDate.HasValue ? $"⏰ Reminder: {ReminderDate:dd MMM yyyy HH:mm}" : "No reminder set";
    public string StatusText => IsCompleted ? "✔ Completed" : "● Pending";
}