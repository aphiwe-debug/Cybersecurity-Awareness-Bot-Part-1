using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CybersecurityBot.Wpf.Models;
using CybersecurityBot.Wpf.Services;

namespace CybersecurityBot.Wpf;

public partial class TasksView : UserControl
{
    private TaskService? _tasks;

    public TasksView()
    {
        InitializeComponent();
    }

    public void Initialize(TaskService tasks) => _tasks = tasks;

    /// <summary>Reloads the list from MySQL.</summary>
    public void Refresh()
    {
        if (_tasks == null) return;
        try
        {
            TaskList.ItemsSource = _tasks.GetAll();
        }
        catch (Exception ex)
        {
            Say("Could not load tasks: " + ex.Message, true);
        }
    }

    private void ReminderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CustomDate == null) return;   // fires once while the XAML is still loading
        CustomDate.Visibility = ReminderBox.SelectedIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
    }

    private DateTime? ReadReminder() => ReminderBox.SelectedIndex switch
    {
        1 => DateTime.Now.AddDays(1),
        2 => DateTime.Now.AddDays(3),
        3 => DateTime.Now.AddDays(7),
        4 => CustomDate.SelectedDate?.Date.AddHours(9),
        _ => null
    };

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (_tasks == null) { Say("The database isn't connected yet.", true); return; }

        string title = TitleBox.Text.Trim();
        string description = DescriptionBox.Text.Trim();

        if (title.Length == 0) { Say("Please enter a title for the task.", true); return; }
        if (description.Length == 0) description = "Cybersecurity task: " + title;

        if (ReminderBox.SelectedIndex == 4)
        {
            if (CustomDate.SelectedDate == null) { Say("Please pick a reminder date.", true); return; }
            if (CustomDate.SelectedDate.Value.Date < DateTime.Today) { Say("The reminder date can't be in the past.", true); return; }
        }

        try
        {
            _tasks.AddTask(title, description, ReadReminder());
            TitleBox.Clear();
            DescriptionBox.Clear();
            ReminderBox.SelectedIndex = 0;
            Refresh();
            Say("✔ Task added.", false);
        }
        catch (Exception ex)
        {
            Say("Database error: " + ex.Message, true);
        }
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e) =>
        RunOnSelected(t => { if (!t.IsCompleted) _tasks!.MarkCompleted(t); }, "✔ Task marked as completed.");

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (TaskList.SelectedItem == null) { Say("Select a task from the list first.", true); return; }
        if (MessageBox.Show("Delete this task permanently?", "Delete task",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

        RunOnSelected(t => _tasks!.Delete(t), "🗑 Task deleted.");
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e) => Refresh();

    private void RunOnSelected(Action<TaskItem> action, string doneMessage)
    {
        if (_tasks == null) { Say("The database isn't connected yet.", true); return; }
        if (TaskList.SelectedItem is not TaskItem task) { Say("Select a task from the list first.", true); return; }

        try
        {
            action(task);
            Refresh();
            Say(doneMessage, false);
        }
        catch (Exception ex)
        {
            Say("Database error: " + ex.Message, true);
        }
    }

    private void Say(string message, bool isError)
    {
        StatusText.Foreground = isError ? Brushes.OrangeRed : Brushes.LightGreen;
        StatusText.Text = message;
    }
}