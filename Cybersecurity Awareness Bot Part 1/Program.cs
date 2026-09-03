using Cybersecurity_Awareness_Bot_Part_1;

namespace CybersecurityChatbot
{

    // PROGRAM: entry point

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. ASCII art — shown immediately, before anything blocking happens
            AsciiArtService.Display();

            // 2. Text-based header
            ConsoleUI.PrintHeader("CYBERSECURITY AWARENESS BOT");

            // 3. Voice greeting — starts playing in the background (non-blocking)
            AudioService.PlayGreeting(Path.Combine("welcome.wav"));

            // 4. Text-based welcome message — types out WHILE the audio plays
            // delay of 60ms roughly paces the text to a ~4-second voice clip; tune to match your actual WAV length
            ConsoleUI.PrintBotMessage("Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.", 60);

            User user = new User();
            while (string.IsNullOrWhiteSpace(user.Name))
            {
                ConsoleUI.PrintBotMessage("Before we start, what's your name?");
                string nameInput = ConsoleUI.PromptUser();

                if (string.IsNullOrWhiteSpace(nameInput))
                {
                    ConsoleUI.PrintError("I didn't catch that. Please type your name.");
                    continue;
                }

                user.Name = nameInput.Trim();
            }

            ConsoleUI.PrintDivider('-');
            ConsoleUI.PrintBotMessage($"Great to meet you, {user.Name}! Let's talk about staying safe online.");
            ConsoleUI.PrintBotMessage("You can ask me about passwords, phishing, or safe browsing. Type 'exit' at any time to quit.");
            ConsoleUI.PrintDivider('-');

            // 5. Main conversation loop
            ResponseService responder = new ResponseService(user);
            bool running = true;

            while (running)
            {
                string input = ConsoleUI.PromptUser(user.Name);

                if (responder.IsInvalid(input))
                {
                    ConsoleUI.PrintError("I didn't quite understand that. Could you rephrase?");
                    continue;
                }

                string trimmed = input.Trim().ToLowerInvariant();
                if (trimmed is "exit" or "quit" or "bye")
                {
                    ConsoleUI.PrintBotMessage($"Goodbye, {user.Name}! Stay safe online.");
                    running = false;
                    continue;
                }

                string response = responder.GetResponse(input);
                ConsoleUI.PrintBotMessage(response);
            }

            ConsoleUI.PrintDivider();
        }
    }
}

