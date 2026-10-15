using System.Windows;

namespace CybersecurityBot.Wpf;

public partial class App : Application
{
    public App()
    {
        // Last line of defence: an unexpected error shows a message instead of crashing the app
        DispatcherUnhandledException += (_, e) =>
        {
            MessageBox.Show("Something unexpected happened, but the bot is still running.",
                            "Cybersecurity Awareness Bot", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        };
    }
}

