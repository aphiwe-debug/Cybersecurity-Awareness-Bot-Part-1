using System.Windows;
using System.Windows.Controls;
using CybersecurityBot.Wpf.Services;

namespace CybersecurityBot.Wpf;

public partial class LogView : UserControl
{
    private const int PageSize = 5;
    private ActivityLog? _log;
    private int _visible = PageSize;

    public LogView()
    {
        InitializeComponent();
    }

    public void Initialize(ActivityLog log)
    {
        _log = log;
        _log.Changed += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        if (_log == null) return;

        int number = 1;
        LogList.ItemsSource = _log.Recent(_visible).Select(entry => $"{number++}. {entry.Display}").ToList();
        CountText.Text = $"Showing {Math.Min(_visible, _log.Count)} of {_log.Count} actions";
        MoreButton.IsEnabled = _log.Count > _visible;
        LessButton.IsEnabled = _visible > PageSize;
    }

    private void MoreButton_Click(object sender, RoutedEventArgs e)
    {
        _visible += PageSize;
        Refresh();
    }

    private void LessButton_Click(object sender, RoutedEventArgs e)
    {
        _visible = Math.Max(PageSize, _visible - PageSize);
        Refresh();
    }
}