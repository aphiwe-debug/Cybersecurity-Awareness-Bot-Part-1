using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Cybersecurity_Awareness_Bot_Part_1
{
    // SERVICE: AudioService — plays the WAV voice greeting
    public static class AudioService
    {
        public static void PlayGreeting(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);

                if (!File.Exists(fullPath))
                {
                    ConsoleUI.PrintError($"Voice greeting not found at '{relativePath}'. Continuing without audio.");
                    return;
                }

                SoundPlayer player = new SoundPlayer(fullPath);
                player.Play(); // non-blocking — audio plays in the background while console output continues
            }
            catch (PlatformNotSupportedException)
            {
                ConsoleUI.PrintError("Voice greeting playback is only supported on Windows. Continuing without audio.");
            }
            catch (Exception ex)
            {
                ConsoleUI.PrintError($"Could not play voice greeting: {ex.Message}");
            }
        }
    }
}
