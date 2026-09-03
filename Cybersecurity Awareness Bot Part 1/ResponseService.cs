using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cybersecurity_Awareness_Bot_Part_1
{
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
}
