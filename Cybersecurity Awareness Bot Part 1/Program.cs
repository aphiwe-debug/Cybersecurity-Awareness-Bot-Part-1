using System.Media;

namespace CybersecurityChatbot
{

    // MODEL: User
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public int QuestionsAsked { get; set; } = 0;
    }

    // SERVICE: ConsoleUI — colours, borders, typing effect
    public static class ConsoleUI
    {
        public static void TypeLine(string text, ConsoleColor color = ConsoleColor.Cyan, int delayMs = 15)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(delayMs);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void PrintDivider(char symbol = '=', int length = 60, ConsoleColor color = ConsoleColor.DarkCyan)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(new string(symbol, length));
            Console.ResetColor();
        }

        public static void PrintHeader(string title)
        {
            PrintDivider();
            Console.ForegroundColor = ConsoleColor.Yellow;
            int padding = Math.Max(0, (60 - title.Length) / 2);
            Console.WriteLine(new string(' ', padding) + title);
            Console.ResetColor();
            PrintDivider();
        }

        public static void PrintBotMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Bot: ");
            Console.ResetColor();
            TypeLine(message, ConsoleColor.White, 8);
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }

        public static string PromptUser(string promptLabel = "You")
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"{promptLabel}: ");
            Console.ResetColor();
            return Console.ReadLine() ?? string.Empty;
        }
    }

    // SERVICE: AsciiArtService — logo displayed on launch

    public static class AsciiArtService
    {
        private const string Logo = @"
   _____ __     __              _____                    _ __       
  / ____|  |   |  |            / ____|                  (_)  |      
 | |    | |   | |__   ___ _ __| (___   ___  ___ _   _ _ __ _| |_ _   _
 | |    | |   | '_ \ / _ \ '__|\___ \ / _ \/ __| | | | '__| | __| | | |
 | |____| |___| |_) |  __/ |   ____) |  __/ (__| |_| | |  | | |_| |_| |
  \_____|______|_.__/ \___|_|  |_____/ \___|\___|\__,_|_|  |_|\__|\__, |
                                                                    __/ |
              A W A R E N E S S   B O T                          |___/ 
";

        public static void Display()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(Logo);
            Console.ResetColor();
        }
    }


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

                using SoundPlayer player = new SoundPlayer(fullPath);
                player.PlaySync(); // waits for playback to finish before continuing
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
