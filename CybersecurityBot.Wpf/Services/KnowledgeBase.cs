namespace CybersecurityBot.Wpf.Services;

/// <summary>All keyword, tip and explanation data lives here so the engine stays small.</summary>
public static class KnowledgeBase
{
    // keyword found in user text -> topic. Order matters: first match wins, so specific phrases come first.
    public static readonly Dictionary<string, string> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["phishing"] = "phishing",
        ["phish"] = "phishing",
        ["suspicious email"] = "phishing",
        ["fake email"] = "phishing",
        ["password"] = "password",
        ["passphrase"] = "password",
        ["2fa"] = "password",
        ["two-factor"] = "password",
        ["scam"] = "scam",
        ["fraud"] = "scam",
        ["con artist"] = "scam",
        ["privacy"] = "privacy",
        ["personal data"] = "privacy",
        ["popia"] = "privacy",
        ["personal information"] = "privacy",
        ["malware"] = "malware",
        ["virus"] = "malware",
        ["ransomware"] = "malware",
        ["antivirus"] = "malware",
        ["browsing"] = "browsing",
        ["browser"] = "browsing",
        ["website"] = "browsing",
        ["link"] = "browsing",
        ["wifi"] = "browsing",
        ["wi-fi"] = "browsing",
    };

    // topic -> several tips (one is picked at random)
    public static readonly Dictionary<string, List<string>> Tips = new()
    {
        ["password"] = new()
        {
            "Use strong, unique passwords for every account. Avoid personal details like birthdays or pet names.",
            "Aim for passphrases of 12+ characters: a mix of random words, numbers and symbols is much harder to crack.",
            "Use a password manager so you never have to reuse or remember dozens of passwords.",
            "Turn on two-factor authentication (2FA) wherever possible. It protects you even if your password leaks."
        },
        ["phishing"] = new()
        {
            "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
            "Check the sender's actual email address, not just the display name, before you trust a message.",
            "Urgent threats like 'your account closes in 24 hours' are a classic phishing trick. Slow down and verify.",
            "Never open unexpected attachments. If in doubt, contact the company through its official website or number."
        },
        ["scam"] = new()
        {
            "If an offer sounds too good to be true, like an unexpected prize or easy money, it's almost certainly a scam.",
            "Never pay upfront fees to claim winnings, loans or job offers. Legitimate organisations don't ask for that.",
            "Banks never ask for your PIN, OTP or full password by phone, SMS or email. Hang up and call the number on your card.",
            "Fake delivery and tax-refund messages are common. Go to the official site yourself instead of clicking the link."
        },
        ["privacy"] = new()
        {
            "Review the privacy settings on your social media and email accounts and limit who can see your personal details.",
            "Only share the personal information a website truly needs. Under POPIA, you have rights over how your data is used.",
            "Think before you post: birthdays, home addresses and holiday plans can help criminals target you.",
            "Avoid logging into sensitive accounts on public Wi-Fi, or use a VPN if you must."
        },
        ["malware"] = new()
        {
            "Keep your operating system, browser and antivirus up to date. Updates patch known security holes.",
            "Only download software and apps from official sources such as the Microsoft Store or Google Play.",
            "Never plug in unknown USB drives. They can install malware the moment they're connected.",
            "Back up important files regularly, ideally offline, so ransomware can't hold your data hostage."
        },
        ["browsing"] = new()
        {
            "Hover over links to preview the real address before clicking, and look for https:// and the padlock icon.",
            "Be careful with pop-ups claiming your device is infected. Close the tab instead of clicking anything.",
            "Type your bank's web address yourself or use its official app instead of following links in messages.",
            "Keep your browser updated and install extensions only from trusted publishers."
        }
    };

    // topic -> longer explanation, used for "tell me more" / "I'm confused"
    public static readonly Dictionary<string, string> Details = new()
    {
        ["password"] = "Attackers use stolen password lists from past breaches and try them on other sites, which is why reuse is so risky. A long passphrase plus 2FA means a leaked password alone isn't enough to get into your account.",
        ["phishing"] = "Phishing works by faking a trusted sender to rush you into clicking or replying. Look at the sender address, hover over links, and notice pressure or threats. When unsure, don't use the message's links: go to the official site yourself.",
        ["scam"] = "Scammers exploit emotion: greed, fear or urgency. They want you to act before thinking. Pause, verify through an official channel, and never send money or codes to someone who contacted you first.",
        ["privacy"] = "Small details add up. A birthday, a school and a street name can be enough for identity theft or to guess security answers. Tighten account settings and share the minimum.",
        ["malware"] = "Malware is software built to harm or spy: viruses, spyware, ransomware. It usually arrives through attachments, fake downloads or infected USB drives, so updates, official sources and backups are your main defence.",
        ["browsing"] = "Unsafe sites can steal logins or push malware. Check the address bar for lookalike domains, prefer https://, and avoid entering personal details on pages you reached through a message or pop-up."
    };
}