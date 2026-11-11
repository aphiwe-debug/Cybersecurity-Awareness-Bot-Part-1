using CybersecurityBot.Wpf.Models;

namespace CybersecurityBot.Wpf.Services;

/// <summary>Question bank (multiple choice and true/false) plus score feedback.</summary>
public static class QuizService
{
    private static readonly List<QuizQuestion> Bank = new()
    {
        new("What should you do if you receive an email asking for your password?",
            new[] { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" }, 2,
            "Reporting phishing emails helps stop scams. Real companies never ask for your password by email."),
        new("True or false: Using the same password on all your accounts is fine as long as it is long.",
            new[] { "True", "False" }, 1,
            "If one site is breached, attackers try that password everywhere. Use a unique password for every account."),
        new("Which of these is the strongest password?",
            new[] { "password123", "Sipho1999", "Tr!ckyKettle-Orbit-47", "qwerty" }, 2,
            "Long passphrases that mix random words, numbers and symbols are the hardest to crack."),
        new("What does two-factor authentication (2FA) add to your account?",
            new[] { "A second way to prove it is you", "A faster login", "A longer password", "Free antivirus" }, 0,
            "2FA means a stolen password alone is not enough to get into your account."),
        new("True or false: A padlock icon in the browser means a website is definitely safe.",
            new[] { "True", "False" }, 1,
            "The padlock only means the connection is encrypted. Scam sites can have one too, so still check the address."),
        new("A caller says they are from your bank and asks for the OTP sent to your phone. What should you do?",
            new[] { "Give it to them, they are verifying you", "Hang up and call your bank on its official number",
                    "Give them only half of the code", "Ask them to email you instead" }, 1,
            "Banks never ask for your OTP. Hang up and call the number on the back of your card."),
        new("True or false: Public Wi-Fi is safe for online banking if the network has a name you recognise.",
            new[] { "True", "False" }, 1,
            "Fake hotspots copy familiar names. Use mobile data or a VPN for anything sensitive."),
        new("What is ransomware?",
            new[] { "Software that speeds up your computer", "Malware that locks your files and demands payment",
                    "A type of firewall", "A spam filter" }, 1,
            "Ransomware encrypts your files and demands money. Regular offline backups are the best defence."),
        new("How can you check where a link goes before you click it?",
            new[] { "Click it and see", "Hover over it to preview the real address",
                    "Trust any shortened link", "Forward it to a friend" }, 1,
            "Hovering shows the real destination. Lookalike addresses are a favourite phishing trick."),
        new("True or false: Software updates often include security fixes, so you should install them promptly.",
            new[] { "True", "False" }, 0,
            "Updates close security holes that attackers already know about."),
        new("Which of these is a warning sign of a phishing email?",
            new[] { "A threat that your account will close in 24 hours unless you click a link", "A newsletter you signed up for",
                    "A message from a colleague you were expecting", "A receipt for something you just bought" }, 0,
            "Urgency and threats are classic phishing tactics designed to make you act before you think."),
        new("True or false: A bank will never ask you for your PIN or full password by phone, SMS or email.",
            new[] { "True", "False" }, 0,
            "Never share your PIN, password or OTP with anyone, even someone claiming to be from your bank.")
    };

    /// <summary>All questions in a fresh random order.</summary>
    public static List<QuizQuestion> NewQuiz() => Bank.OrderBy(_ => Random.Shared.Next()).ToList();

    public static string Feedback(int score, int total)
    {
        double percent = score * 100.0 / total;
        if (percent >= 80) return "Great job! You're a cybersecurity pro!";
        if (percent >= 50) return "Good effort! A little more practice and you'll be a pro.";
        return "Keep learning to stay safe online!";
    }
}