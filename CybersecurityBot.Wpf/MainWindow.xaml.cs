using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CybersecurityBot.Wpf.Models;
using CybersecurityBot.Wpf.Services;

namespace CybersecurityBot.Wpf;

public partial class MainWindow : Window
{
    private readonly User _user = new();
    private readonly TaskService _tasks;
    private readonly DispatcherTimer _reminderTimer = new() { Interval = TimeSpan.FromSeconds(20) };
    private readonly HashSet<int> _notified = new();   // reminders already shown this session
    private bool _awaitingName = true;
    private bool _botBusy;
    private bool _dbReady;
    private ResponseEngine? _engine;
    private AssistantService? _assistant;

    public DatabaseService Db { get; } = new(DatabaseSettings.Load());
    public ActivityLog Log { get; } = new();

    public MainWindow()
    {
        InitializeComponent();
        _tasks = new TaskService(Db, Log.Add);
        TasksPage.Initialize(_tasks);
        QuizPage.Initialize(Log);
        LogPage.Initialize(Log);

        _reminderTimer.Tick += (_, _) => CheckReminders();
        _reminderTimer.Start();
        Loaded += async (_, _) => await StartAsync();
    }

    private async Task StartAsync()
    {
        SetInputEnabled(false);
        LogoText.Text = AsciiArtService.Logo.Trim('\r', '\n');

        string? audioError = AudioService.PlayGreeting("welcome.wav");
        if (audioError != null)
            AddBubble("System", "⚠ " + audioError, "#5A1D1D", HorizontalAlignment.Center);

        string? dbError = await Task.Run(() => Db.Initialize());
        _dbReady = dbError == null;
        AddBubble("System",
            dbError == null ? "✔ Connected to the MySQL database." : "⚠ " + dbError,
            dbError == null ? "#1B4332" : "#5A1D1D",
            HorizontalAlignment.Center);
        if (_dbReady) TasksPage.Refresh();

        await BotSayAsync("Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.", 30);
        await BotSayAsync("Before we start, what's your name?");
        SetInputEnabled(true);
    }

    // ---------- Event handlers ----------
    private async void SendButton_Click(object sender, RoutedEventArgs e) => await HandleSendAsync();

    private async void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) await HandleSendAsync();
    }

    // ---------- Conversation flow ----------
    private async Task HandleSendAsync()
    {
        if (_botBusy) return;

        string input = InputBox.Text.Trim();
        InputBox.Clear();

        // Input validation: empty entries
        if (string.IsNullOrWhiteSpace(input))
        {
            AddBubble("System", "⚠ I didn't catch that. Please type something first.", "#5A1D1D", HorizontalAlignment.Center);
            return;
        }

        AddBubble(_awaitingName ? "You" : _user.Name, input, "#30363D", HorizontalAlignment.Right);

        if (_awaitingName)
        {
            await HandleNameAsync(input);
            return;
        }

        if (input.ToLowerInvariant() is "exit" or "quit" or "bye")
        {
            SetInputEnabled(false);
            await BotSayAsync($"Goodbye, {_user.Name}! Stay safe online.");
            await Task.Delay(1200);
            Close();
            return;
        }

        await BotSayAsync(GetReply(input));
    }

    private async Task HandleNameAsync(string input)
    {
        // Accepts "Aphiwe" as well as "my name is Aphiwe"
        string name = Regex.Replace(input, @"^(my name is|i am|i'm|im)\s+", "", RegexOptions.IgnoreCase).Trim();

        if (name.Length == 0 || name.Length > 30)
        {
            await BotSayAsync("Please enter just your first name (30 characters or fewer).");
            return;
        }

        _user.Name = name;
        _awaitingName = false;
        _engine = new ResponseEngine(_user);
        _assistant = new AssistantService(_tasks, Log, OpenQuiz, () => TasksPage.Refresh());
        Log.Add($"Chat started with {name}");

        await BotSayAsync($"Great to meet you, {name}! Let's talk about staying safe online.");
        await BotSayAsync("Ask me about passwords, phishing, scams or privacy. You can also say 'add task - review privacy settings', 'remind me to update my password tomorrow', 'start quiz' or 'show activity log'. Type 'exit' to quit.");
    }

    /// <summary>Commands (tasks, quiz, log) get first chance, then the Part 2 chat engine answers.</summary>
    private string GetReply(string input) => _assistant!.TryHandle(input) ?? _engine!.GetResponse(input);

    private void OpenQuiz()
    {
        MainTabs.SelectedItem = QuizTab;
        QuizPage.StartQuiz();
    }

    // ---------- Reminder alerts ----------
    private void CheckReminders()
    {
        if (!_dbReady || _awaitingName || _botBusy) return;

        try
        {
            var due = _tasks.GetAll()
                .Where(t => !t.IsCompleted && t.ReminderDate.HasValue && t.ReminderDate <= DateTime.Now && !_notified.Contains(t.Id))
                .ToList();

            foreach (TaskItem task in due)
            {
                _notified.Add(task.Id);
                Log.Add($"Reminder due: '{task.Title}'");
                AddBubble("⏰ Reminder", $"{task.Title}\n{task.Description}", "#9A6700", HorizontalAlignment.Left);
            }

            if (due.Count > 0)
            {
                SystemSounds.Exclamation.Play();
                TasksPage.Refresh();
            }
        }
        catch
        {
            // database unavailable: try again on the next tick
        }
    }

    // ---------- UI helpers ----------
    private async Task BotSayAsync(string text, int delayMs = 12)
    {
        if (text.Length > 120) delayMs = 3;   // long replies type faster

        _botBusy = true;
        TextBlock body = AddBubble("Bot", string.Empty, "#1F6FEB", HorizontalAlignment.Left);

        foreach (char c in text)
        {
            body.Text += c;
            ChatScroll.ScrollToEnd();
            await Task.Delay(delayMs);
        }

        _botBusy = false;
        InputBox.Focus();
    }

    private TextBlock AddBubble(string sender, string text, string backgroundHex, HorizontalAlignment align)
    {
        var label = new TextBlock
        {
            Text = sender,
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = Hex("#C9D1D9"),
            Margin = new Thickness(0, 0, 0, 2)
        };
        var body = new TextBlock
        {
            Text = text,
            FontSize = 14,
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap
        };
        var stack = new StackPanel();
        stack.Children.Add(label);
        stack.Children.Add(body);

        ChatPanel.Children.Add(new Border
        {
            Background = Hex(backgroundHex),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12, 8, 12, 8),
            Margin = new Thickness(4),
            MaxWidth = 600,
            HorizontalAlignment = align,
            Child = stack
        });
        ChatScroll.ScrollToEnd();
        return body;
    }

    private void SetInputEnabled(bool enabled)
    {
        InputBox.IsEnabled = enabled;
        SendButton.IsEnabled = enabled;
        if (enabled) InputBox.Focus();
    }

    private static Brush Hex(string hex) => (Brush)new BrushConverter().ConvertFrom(hex)!;
}