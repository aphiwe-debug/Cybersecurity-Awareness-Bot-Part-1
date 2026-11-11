using System.IO;
using System.Text.Json;

namespace CybersecurityBot.Wpf.Services;

/// <summary>Connection details, loaded from dbsettings.json so the password is never committed to Git.</summary>
public class DatabaseSettings
{
    public string Server { get; set; } = "localhost";
    public uint Port { get; set; } = 3306;
    public string User { get; set; } = "root";
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = "cybersecurity_bot";

    public static DatabaseSettings Load()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "dbsettings.json");
        if (!File.Exists(path)) return new DatabaseSettings();   // fall back to defaults

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<DatabaseSettings>(File.ReadAllText(path), options)
                   ?? new DatabaseSettings();
        }
        catch (JsonException)
        {
            return new DatabaseSettings();
        }
    }
}