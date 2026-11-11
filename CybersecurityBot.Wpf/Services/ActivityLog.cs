namespace CybersecurityBot.Wpf.Services;

public record LogEntry(DateTime Time, string Text)
{
    public string Display => $"{Time:dd MMM HH:mm:ss}  |  {Text}";
}

/// <summary>In-memory record of what the bot has done. Raises Changed so the Log tab can update itself.</summary>
public class ActivityLog
{
    private readonly List<LogEntry> _entries = new();

    public event Action? Changed;
    public int Count => _entries.Count;

    public void Add(string text)
    {
        _entries.Add(new LogEntry(DateTime.Now, text));
        Changed?.Invoke();
    }

    /// <summary>Newest first.</summary>
    public List<LogEntry> Recent(int count) => _entries.AsEnumerable().Reverse().Take(count).ToList();

    /// <summary>Numbered text for the chat, newest first.</summary>
    public string Summary(int count)
    {
        if (_entries.Count == 0)
            return "I haven't done anything for you yet. Try adding a task or starting the quiz!";

        var lines = Recent(count).Select((e, i) => $"{i + 1}. {e.Text} ({e.Time:dd MMM HH:mm})");
        string text = "Here's a summary of recent actions:\n" + string.Join("\n", lines);
        if (_entries.Count > count)
            text += $"\n(Showing {count} of {_entries.Count}. Say 'show more' to see more.)";
        return text;
    }
}