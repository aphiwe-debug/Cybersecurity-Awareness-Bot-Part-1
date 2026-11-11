using System.Globalization;
using System.Text.RegularExpressions;
using CybersecurityBot.Wpf.Models;

namespace CybersecurityBot.Wpf.Services;

/// <summary>
/// NLP simulation: works out what the user wants from free text using regular expressions and
/// string matching. Returns null when the message isn't a command, so the normal chat engine answers.
/// </summary>
public class AssistantService
{
    private const RegexOptions Opts = RegexOptions.IgnoreCase | RegexOptions.Compiled;

    // "add a task to ...", "create task - ...", "add a reminder to ..."
    private static readonly Regex AddTaskPattern = new(
        @"^(?:(?:can|could|would) you |please |i want to |i need to |i'd like to )*(?:add|create|make|set up|new)\s+(?:a |an |another |new )*(?<kind>task|todo|to-do|reminder)\b\s*(?:to|for|about|called|named|-|:)?\s*(?<title>.*)$", Opts);

    // "remind me to ...", "can you remind me to ...", "set a reminder to ..."
    private static readonly Regex RemindPattern = new(
        @"^(?:(?:can|could|would) you |please )*(?:remind me|set (?:me )?a reminder|set reminder)\b\s*(?:to|about|for|that|-|:)?\s*(?<title>.*)$", Opts);

    private static readonly Regex QuizPattern = new(@"\b(quiz|trivia|test my knowledge|play a game|mini.?game)\b", Opts);
    private static readonly Regex LogPattern = new(@"activity log|what have you done|what did you do|recent actions|show (?:me )?(?:the )?log|your log", Opts);
    private static readonly Regex MorePattern = new(@"^(?:show |see |give me )?(?:some )?more(?: actions| log| activity)?[.!]?$", Opts);
    private static readonly Regex ShowTasksPattern = new(@"\b(?:show|view|list|see|display)\b.*\b(?:tasks?|to-?dos?|reminders?)\b|what (?:are )?my tasks|my tasks", Opts);

    private static readonly Regex InPattern = new(
        @"\bin (?<n>\d+|an?|one|two|three|four|five|six|seven|eight|nine|ten)\s+(?<unit>hour|day|week|month)s?\b", Opts);
    private static readonly Regex WeekdayPattern = new(
        @"\b(?:on |next )?(?<day>monday|tuesday|wednesday|thursday|friday|saturday|sunday)\b", Opts);

    private static readonly Dictionary<string, int> Numbers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["a"] = 1,
        ["an"] = 1,
        ["one"] = 1,
        ["two"] = 2,
        ["three"] = 3,
        ["four"] = 4,
        ["five"] = 5,
        ["six"] = 6,
        ["seven"] = 7,
        ["eight"] = 8,
        ["nine"] = 9,
        ["ten"] = 10
    };

    private static readonly (Regex Pattern, Func<DateTime> When)[] KeywordDates =
    {
        (new Regex(@"\bnext week\b", Opts), () => DateTime.Now.AddDays(7)),
        (new Regex(@"\bnext month\b", Opts), () => DateTime.Now.AddMonths(1)),
        (new Regex(@"\btomorrow\b", Opts), () => DateTime.Today.AddDays(1).AddHours(9)),
        (new Regex(@"\btoday\b", Opts), () => DateTime.Now.AddHours(1))
    };

    // keyword in the task title -> a helpful description
    private static readonly (string Keyword, string Description)[] TaskDescriptions =
    {
        ("two-factor", "Turn on two-factor authentication so a stolen password alone can't unlock your account."),
        ("2fa", "Turn on two-factor authentication so a stolen password alone can't unlock your account."),
        ("privacy", "Review account privacy settings to ensure your data is protected."),
        ("password", "Create a strong, unique password and store it in a password manager."),
        ("backup", "Back up your important files, keeping one copy offline."),
        ("update", "Install pending software and security updates."),
        ("antivirus", "Make sure your antivirus is installed, active and up to date."),
        ("wifi", "Check your Wi-Fi and router security settings.")
    };

    private readonly TaskService _tasks;
    private readonly ActivityLog _log;
    private readonly Action _openQuiz;
    private readonly Action _tasksChanged;

    private TaskItem? _pendingReminderTask;   // waiting for the answer to "Would you like a reminder?"
    private bool _lastWasLog;                 // lets "show more" continue the log

    public AssistantService(TaskService tasks, ActivityLog log, Action openQuiz, Action tasksChanged)
    {
        _tasks = tasks;
        _log = log;
        _openQuiz = openQuiz;
        _tasksChanged = tasksChanged;
    }

    public string? TryHandle(string input)
    {
        try
        {
            return Route(input.Trim());
        }
        catch (Exception ex)
        {
            return "⚠ I couldn't do that because of a database problem. Check that MySQL is running. (" + ex.Message + ")";
        }
    }

    private string? Route(string text)
    {
        string lower = text.ToLowerInvariant();
        bool wasLog = _lastWasLog;
        _lastWasLog = false;

        string? pending = HandlePendingReminder(lower);
        if (pending != null) return pending;

        if (LogPattern.IsMatch(lower))
        {
            string summary = _log.Summary(5);
            _log.Add("NLP: understood a request to show the activity log");
            _lastWasLog = true;
            return summary;
        }

        if (wasLog && MorePattern.IsMatch(lower))
            return _log.Summary(Math.Min(_log.Count, 25));

        if (QuizPattern.IsMatch(lower))
        {
            _log.Add($"NLP: understood '{text}' as a request to start the quiz");
            _openQuiz();
            return "Starting the cybersecurity quiz! I've opened the Quiz tab for you. Good luck!";
        }

        Match add = AddTaskPattern.Match(text);
        if (add.Success)
            return HandleAddTask(text, add.Groups["title"].Value, add.Groups["kind"].Value.Equals("reminder", StringComparison.OrdinalIgnoreCase));

        Match remind = RemindPattern.Match(text);
        if (remind.Success)
            return HandleAddTask(text, remind.Groups["title"].Value, true);

        if (ShowTasksPattern.IsMatch(lower))
            return ShowTasks();

        return null;
    }

    private string HandleAddTask(string original, string rawTitle, bool isReminderRequest)
    {
        DateTime? due = ExtractDate(rawTitle, out string withoutDate);
        string title = CleanTitle(withoutDate);

        if (title.Length == 0)
            return "What should the task be? Try: 'Add task - Review privacy settings'.";

        _log.Add($"NLP: understood '{original}' as a request to add a task" + (due.HasValue ? " with a reminder" : ""));
        TaskItem task = _tasks.AddTask(title, DescribeTask(title), due);
        _tasksChanged();

        if (due.HasValue)
            return $"Reminder set for '{title}' on {FriendlyDate(due.Value)}. I've saved it to your task list.";

        _pendingReminderTask = task;
        return isReminderRequest
            ? $"Task added: '{title}'. When should I remind you? For example 'tomorrow' or 'in 3 days'."
            : $"Task added: '{title}'. Would you like a reminder?";
    }

    /// <summary>Handles the answer to "Would you like a reminder?"</summary>
    private string? HandlePendingReminder(string lower)
    {
        if (_pendingReminderTask == null) return null;
        TaskItem task = _pendingReminderTask;

        if (Regex.IsMatch(lower, @"^(no|nope|nah|no thanks|no thank you|not now|skip)\b"))
        {
            _pendingReminderTask = null;
            return "No problem, the task is saved without a reminder.";
        }

        DateTime? due = ExtractDate(lower, out _);
        if (due.HasValue)
        {
            _pendingReminderTask = null;
            _log.Add($"NLP: understood '{lower}' as a reminder for '{task.Title}'");
            _tasks.SetReminder(task, due.Value);
            _tasksChanged();
            return $"Got it! I'll remind you on {FriendlyDate(due.Value)}.";
        }

        if (Regex.IsMatch(lower, @"^(yes|yeah|yep|sure|ok|okay|please)\b"))
            return "When should I remind you? For example 'tomorrow', 'in 3 days' or 'next week'.";

        _pendingReminderTask = null;   // the user moved on to something else
        return null;
    }

    private string ShowTasks()
    {
        var pending = _tasks.GetAll().Where(t => !t.IsCompleted).Take(8).ToList();
        _log.Add("NLP: understood a request to list tasks");

        if (pending.Count == 0)
            return "You have no pending tasks. Try 'Add task - Review privacy settings'.";

        var lines = pending.Select((t, i) =>
            $"{i + 1}. {t.Title} ({(t.ReminderDate.HasValue ? "reminder " + FriendlyDate(t.ReminderDate.Value) : "no reminder")})");
        return "Here are your pending tasks:\n" + string.Join("\n", lines) + "\nYou can complete or delete them in the Tasks tab.";
    }

    // ---------- Helpers ----------
    /// <summary>Finds a date phrase such as "tomorrow" or "in 3 days" and removes it from the text.</summary>
    private static DateTime? ExtractDate(string text, out string remaining)
    {
        remaining = text;
        DateTime now = DateTime.Now;

        Match m = InPattern.Match(text);
        if (m.Success)
        {
            string raw = m.Groups["n"].Value;
            int n = int.TryParse(raw, out int parsed) ? parsed : Numbers.GetValueOrDefault(raw, 1);
            DateTime due = m.Groups["unit"].Value.ToLowerInvariant() switch
            {
                "hour" => now.AddHours(n),
                "week" => now.AddDays(7 * n),
                "month" => now.AddMonths(n),
                _ => now.AddDays(n)
            };
            remaining = text.Remove(m.Index, m.Length);
            return due;
        }

        foreach (var (pattern, when) in KeywordDates)
        {
            Match k = pattern.Match(text);
            if (k.Success)
            {
                remaining = text.Remove(k.Index, k.Length);
                return when();
            }
        }

        Match wd = WeekdayPattern.Match(text);
        if (wd.Success)
        {
            var target = Enum.Parse<DayOfWeek>(wd.Groups["day"].Value, true);
            int days = ((int)target - (int)now.DayOfWeek + 7) % 7;
            if (days == 0) days = 7;
            remaining = text.Remove(wd.Index, wd.Length);
            return now.Date.AddDays(days).AddHours(9);
        }

        return null;
    }

    private static string CleanTitle(string raw)
    {
        string t = Regex.Replace(raw, @"\s+", " ").Trim().TrimEnd('.', '?', '!', ',', ' ');
        t = Regex.Replace(t, @"^(?:to|that|about|for|on|at)\s+", "", RegexOptions.IgnoreCase).Trim();
        t = Regex.Replace(t, @"\s+(?:on|at|by|for)$", "", RegexOptions.IgnoreCase).Trim();
        if (t.Length == 0) return t;
        if (t.Length > 150) t = t[..150];
        return char.ToUpper(t[0]) + t[1..];
    }

    private static string DescribeTask(string title)
    {
        foreach (var (keyword, description) in TaskDescriptions)
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase)) return description;
        return $"Cybersecurity task: {title}.";
    }

    private static string FriendlyDate(DateTime due) =>
        due.Date == DateTime.Today.AddDays(1)
            ? $"tomorrow ({due:dd MMM yyyy})"
            : due.ToString("dd MMM yyyy HH:mm", CultureInfo.CurrentCulture);
}