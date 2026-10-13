using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CybersecurityBot.Wpf.Models;

namespace CybersecurityBot.Wpf;

public partial class MainWindow : Window
{
    private readonly User _user = new();
    private bool _awaitingName = true;
    private bool _botBusy;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await StartAsync();
    }

    private async Task StartAsync()
    {
        SetInputEnabled(false);
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
        await BotSayAsync($"Great to meet you, {name}! Let's talk about staying safe online.");
        await BotSayAsync("You can ask me about passwords, phishing, scams, privacy or safe browsing. Type 'exit' to quit.");
    }

    private string GetReply(string input) => "You can ask me about passwords, phishing, or safe browsing."; // replaced in Step 6

    // ---------- UI helpers ----------
    private async Task BotSayAsync(string text, int delayMs = 12)
    {
        _botBusy = true;
        TextBlock body = AddBubble("Bot", string.Empty, "#1F6FEB", HorizontalAlignment.Left);

        foreach (char c in text)   // typing effect, same idea as Part 1's TypeLine
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