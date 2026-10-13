using System.IO;
using System.Media;

namespace CybersecurityBot.Wpf.Services;

public static class AudioService
{
    private static SoundPlayer? _player;   // kept in a field so it isn't garbage-collected mid-playback

    /// <summary>Plays the WAV greeting. Returns null on success, or an error message.</summary>
    public static string? PlayGreeting(string fileName)
    {
        try
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (!File.Exists(fullPath))
                return $"Voice greeting not found ('{fileName}'). Continuing without audio.";

            _player = new SoundPlayer(fullPath);
            _player.Play();   // non-blocking
            return null;
        }
        catch (Exception ex)
        {
            return $"Could not play voice greeting: {ex.Message}";
        }
    }
}