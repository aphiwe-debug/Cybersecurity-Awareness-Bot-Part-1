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
    // SERVICE: ResponseService — cybersecurity Q&A + input validation

    public class ResponseService
    {
        private readonly User _user;

        public ResponseService(User user)
        {
            _user = user;
        }

        public bool IsInvalid(string input) => string.IsNullOrWhiteSpace(input);

        public string GetResponse(string input)
        {
            _user.QuestionsAsked++;
            string text = input.Trim().ToLowerInvariant();

            if (text is "exit" or "quit" or "bye")
                return $"Goodbye, {_user.Name}! Stay safe online.";

            if (Contains(text, "how are you"))
                return "I'm running smoothly and ready to help you stay cyber-safe today!";

            if (Contains(text, "purpose") || Contains(text, "what do you do"))
                return "My purpose is to help educate South African citizens about cybersecurity threats like phishing, malware, and unsafe browsing — so you can protect yourself online.";

            if (Contains(text, "what can i ask") || Contains(text, "help") || Contains(text, "topics"))
                return "You can ask me about: password safety, phishing emails, safe browsing, malware, and general online safety tips.";

            if (Contains(text, "password"))
                return "Password safety tip: use at least 12 characters, mix upper/lower case, numbers and symbols, never reuse passwords across sites, and consider a password manager.";

            if (Contains(text, "phishing") || Contains(text, "scam email") || Contains(text, "suspicious email"))
                return "Phishing tip: be wary of emails urging urgent action, check the sender's actual address (not just the display name), and never click links or download attachments from unknown senders.";

            if (Contains(text, "link") || Contains(text, "browsing") || Contains(text, "browser") || Contains(text, "website"))
                return "Safe browsing tip: hover over links to preview the real URL before clicking, look for 'https://' and a padlock icon, and avoid entering personal details on unfamiliar sites.";

            if (Contains(text, "malware") || Contains(text, "virus"))
                return "Malware tip: keep your antivirus and operating system updated, avoid downloading software from untrusted sources, and don't plug in unknown USB drives.";

            if (Contains(text, "name") && Contains(text, "your"))
                return "I'm the Cybersecurity Awareness Bot — you can just call me Bot!";

            return "I didn't quite understand that. Could you rephrase? You can ask me about passwords, phishing, or safe browsing.";
        }

        private static bool Contains(string haystack, string needle) => haystack.Contains(needle, StringComparison.OrdinalIgnoreCase);
    }

