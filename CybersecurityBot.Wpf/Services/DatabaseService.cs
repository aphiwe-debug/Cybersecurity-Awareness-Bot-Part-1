using MySql.Data.MySqlClient;

namespace CybersecurityBot.Wpf.Services;

/// <summary>Creates the database and tasks table on first run and hands out connections.</summary>
public class DatabaseService
{
    private readonly DatabaseSettings _settings;

    public DatabaseService(DatabaseSettings settings)
    {
        _settings = settings;
    }

    private string BuildConnectionString(bool includeDatabase)
    {
        var builder = new MySqlConnectionStringBuilder
        {
            Server = _settings.Server,
            Port = _settings.Port,
            UserID = _settings.User,
            Password = _settings.Password,
            ConnectionTimeout = 5   // fail fast if the server isn't running
        };
        if (includeDatabase) builder.Database = _settings.Database;
        return builder.ConnectionString;
    }

    /// <summary>Opens a connection to the bot's database. The caller disposes it.</summary>
    public MySqlConnection OpenConnection()
    {
        var connection = new MySqlConnection(BuildConnectionString(includeDatabase: true));
        connection.Open();
        return connection;
    }

    /// <summary>Creates the database and tables if missing. Returns null on success, or a friendly error message.</summary>
    public string? Initialize()
    {
        try
        {
            using (var server = new MySqlConnection(BuildConnectionString(includeDatabase: false)))
            {
                server.Open();
                using var createDb = new MySqlCommand(
                    $"CREATE DATABASE IF NOT EXISTS `{_settings.Database}` CHARACTER SET utf8mb4;", server);
                createDb.ExecuteNonQuery();
            }

            using var db = OpenConnection();
            using var createTable = new MySqlCommand(@"
                CREATE TABLE IF NOT EXISTS tasks (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    title VARCHAR(150) NOT NULL,
                    description VARCHAR(500) NOT NULL,
                    reminder_date DATETIME NULL,
                    is_completed TINYINT(1) NOT NULL DEFAULT 0,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                );", db);
            createTable.ExecuteNonQuery();
            return null;
        }
        catch (MySqlException ex) when (ex.Number == 1045)
        {
            return "MySQL refused the login. Check the User and Password in dbsettings.json.";
        }
        catch (MySqlException ex) when (ex.Number is 0 or 1042)
        {
            return "Could not reach the MySQL server. Check that MySQL is running on port " + _settings.Port + ".";
        }
        catch (Exception ex)
        {
            return "Database error: " + ex.Message;
        }
    }
}