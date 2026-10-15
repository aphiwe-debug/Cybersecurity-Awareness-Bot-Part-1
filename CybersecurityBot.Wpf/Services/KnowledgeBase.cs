namespace CybersecurityBot.Wpf.Services;

/// <summary>All keyword, tip, explanation and small-talk data lives here so the engine stays small.</summary>
public static class KnowledgeBase
{
    // keyword found in user text -> topic. First match wins, so specific phrases come before generic ones.
    public static readonly Dictionary<string, string> Keywords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["phishing"] = "phishing",
        ["phish"] = "phishing",
        ["suspicious email"] = "phishing",
        ["fake email"] = "phishing",
        ["social engineering"] = "social engineering",
        ["vishing"] = "social engineering",
        ["smishing"] = "social engineering",
        ["impersonat"] = "social engineering",
        ["pretend"] = "social engineering",
        ["password"] = "password",
        ["passphrase"] = "password",
        ["2fa"] = "password",
        ["two-factor"] = "password",
        ["two factor"] = "password",
        ["scam"] = "scam",
        ["fraud"] = "scam",
        ["con artist"] = "scam",
        ["identity"] = "identity",
        ["sim swap"] = "identity",
        ["id theft"] = "identity",
        ["banking"] = "banking",
        ["bank"] = "banking",
        ["credit card"] = "banking",
        ["debit card"] = "banking",
        ["one-time pin"] = "banking",
        ["one time pin"] = "banking",
        ["privacy"] = "privacy",
        ["personal data"] = "privacy",
        ["popia"] = "privacy",
        ["personal information"] = "privacy",
        ["wifi"] = "wifi",
        ["wi-fi"] = "wifi",
        ["vpn"] = "wifi",
        ["hotspot"] = "wifi",
        ["malware"] = "malware",
        ["virus"] = "malware",
        ["ransomware"] = "malware",
        ["antivirus"] = "malware",
        ["update"] = "updates",
        ["patch"] = "updates",
        ["backup"] = "updates",
        ["back up"] = "updates",
        ["back-up"] = "updates",
        ["browsing"] = "browsing",
        ["browser"] = "browsing",
        ["website"] = "browsing",
        ["link"] = "browsing",
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
            "Check which apps have access to your camera, microphone and location, and remove the ones that don't need it."
        },
        ["malware"] = new()
        {
            "Keep your operating system, browser and antivirus up to date. Updates patch known security holes.",
            "Only download software and apps from official sources such as the Microsoft Store or Google Play.",
            "Never plug in unknown USB drives. They can install malware the moment they're connected.",
            "Be wary of 'free' cracked software and games. They are a common way to smuggle in ransomware."
        },
        ["browsing"] = new()
        {
            "Hover over links to preview the real address before clicking, and look for https:// and the padlock icon.",
            "Be careful with pop-ups claiming your device is infected. Close the tab instead of clicking anything.",
            "Type your bank's web address yourself or use its official app instead of following links in messages.",
            "Watch for lookalike addresses such as 'standardbnak.co.za'. One swapped letter is enough to fool you."
        },
        ["social engineering"] = new()
        {
            "Social engineers manipulate people, not computers. Be suspicious of anyone who creates urgency or claims authority, such as 'your bank', 'IT support' or 'the police'.",
            "Never share OTPs, PINs or passwords with a caller, even if they already know some of your details. Hang up and phone the official number yourself.",
            "Verify before you trust: confirm unusual requests through a second channel, like calling a colleague on their known number."
        },
        ["wifi"] = new()
        {
            "Public Wi-Fi at malls, airports and cafes isn't private. Avoid banking or shopping on it, or switch to your mobile data.",
            "Use a reputable VPN on public networks. It encrypts your traffic so others on the network can't read it.",
            "Turn off auto-connect to open networks and forget networks you no longer use. Fake hotspots often copy names like 'Free_WiFi'.",
            "At home, change your router's default admin password and use WPA2 or WPA3 encryption."
        },
        ["identity"] = new()
        {
            "Shred or securely delete documents showing your ID number, bank details or address. Don't post photos of your ID, card or boarding pass online.",
            "Check your credit report regularly for accounts you didn't open. In South Africa you can request a free report from each credit bureau once a year.",
            "If your phone suddenly loses signal for no reason, contact your network provider immediately. It can be a sign of a SIM swap."
        },
        ["banking"] = new()
        {
            "Only use your bank's official app or type the web address yourself. Never log in through a link in an SMS or email.",
            "Turn on transaction notifications and set sensible limits so unusual activity stands out quickly.",
            "Never share your PIN, password or OTP with anyone, including people claiming to be from your bank. Banks never ask for these."
        },
        ["updates"] = new()
        {
            "Install software and phone updates promptly. Many of them fix security holes attackers already know about.",
            "Turn on automatic updates for your operating system, browser and apps.",
            "Follow the 3-2-1 backup idea: 3 copies of your files, on 2 different types of storage, with 1 kept offline or off-site."
        }
    };

    // topic -> longer explanation, used for "tell me more" / "I'm confused" / "what is X"
    public static readonly Dictionary<string, string> Details = new()
    {
        ["password"] = "Attackers use stolen password lists from past breaches and try them on other sites, which is why reuse is so risky. A long passphrase plus 2FA means a leaked password alone isn't enough to get into your account.",
        ["phishing"] = "Phishing works by faking a trusted sender to rush you into clicking or replying. Look at the sender address, hover over links, and notice pressure or threats. When unsure, don't use the message's links: go to the official site yourself.",
        ["scam"] = "Scammers exploit emotion: greed, fear or urgency. They want you to act before thinking. Pause, verify through an official channel, and never send money or codes to someone who contacted you first.",
        ["privacy"] = "Small details add up. A birthday, a school and a street name can be enough for identity theft or to guess security answers. Tighten account settings and share the minimum.",
        ["malware"] = "Malware is software built to harm or spy: viruses, spyware, ransomware. It usually arrives through attachments, fake downloads or infected USB drives, so updates, official sources and backups are your main defence.",
        ["browsing"] = "Unsafe sites can steal logins or push malware. Check the address bar for lookalike domains, prefer https://, and avoid entering personal details on pages you reached through a message or pop-up.",
        ["social engineering"] = "Social engineering exploits trust, fear and helpfulness. Attackers research you on social media, then pose as a bank agent, courier or manager. Their favourite tools are urgency, authority and a believable story. A short pause and an independent check beat almost every attempt.",
        ["wifi"] = "On an open network, anyone nearby can try to snoop or set up a fake hotspot with a trusted-looking name. Encrypted sites (https) protect a lot, but a VPN or your own mobile data is safer for anything sensitive.",
        ["identity"] = "Identity thieves piece together your ID number, address and bank details from leaks, social media and discarded paperwork, then open accounts or take over yours. Limit what you share, monitor your credit report, and act fast on any sign of misuse.",
        ["banking"] = "Most online banking fraud tricks the customer, not the bank's systems: fake bank pages, calls from fake agents and malicious apps. Using the official app, keeping notifications on and refusing to share OTPs closes most of those doors. If you suspect fraud, call the number on the back of your card straight away.",
        ["updates"] = "Attackers scan for devices running old software because known flaws are easy to exploit. Updates close those holes, and backups mean that even if something goes wrong, like ransomware, you don't lose your files."
    };

    // topic -> real-world example, the next step after the explanation
    public static readonly Dictionary<string, string> Examples = new()
    {
        ["password"] = "Example: a gaming site is breached and your password leaks. An attacker tries the same email and password on your email and banking. If you reused it, they're in within minutes.",
        ["phishing"] = "Example: you get an email that looks like it's from your courier: 'Parcel on hold, pay a R25 customs fee'. The link leads to a fake payment page that steals your card details.",
        ["scam"] = "Example: a WhatsApp message says you've won a competition, but you must pay a R500 'release fee' first. After you pay, they ask for more, and the prize never existed.",
        ["privacy"] = "Example: you post your birthday, your school and your pet's name. These are the same answers many security questions use, so an attacker can reset your account.",
        ["malware"] = "Example: you download a free 'cracked' game from an unofficial site. It comes bundled with ransomware that encrypts your files and demands payment.",
        ["browsing"] = "Example: you search for your bank and click the first ad. It's a lookalike domain, and the page records your details the moment you sign in.",
        ["social engineering"] = "Example: someone calls claiming to be from your bank's fraud department, reads out part of your card number, and asks for the OTP you just received to 'cancel a fraudulent transaction'. That OTP actually approves the transfer. The real bank never asks for it.",
        ["wifi"] = "Example: you connect to 'Airport_Free_WiFi' and log in to your email. The hotspot was set up by an attacker near the gate, and they capture your login details.",
        ["identity"] = "Example: a fraudster uses a leaked ID number and a fake affidavit to get a SIM swap. Your phone loses signal, and the attacker receives your banking OTPs.",
        ["banking"] = "Example: an SMS says your account is locked and links to a page that looks exactly like your bank's login. Once you type your details, the criminal logs in on the real site with them.",
        ["updates"] = "Example: in 2017 the WannaCry ransomware spread through Windows computers that had skipped a patch released about two months earlier, locking files worldwide."
    };

    // ---------- Variety ----------
    public static readonly string[] TipIntros =
    {
        "Here's something worth knowing:",
        "Quick tip:",
        "Good one to ask about. Try this:",
        "One thing I always recommend:",
        "Here's a practical tip:"
    };

    public static readonly string[] Fallbacks =
    {
        "I'm not sure I understand that one.",
        "Hmm, that's outside what I know right now.",
        "I didn't quite catch that, could you rephrase it?"
    };

    // regex pattern -> several replies ({name} is replaced with the user's name)
    public static readonly List<(string Pattern, string[] Replies)> SmallTalk = new()
    {
        (@"\b(hi|hello|hey|howzit)\b", new[]
        {
            "Hello {name}! Great to see you. What would you like to learn about today?",
            "Hey {name}! Ready to level up your online safety?",
            "Howzit {name}! Ask me about passwords, scams, privacy, or anything else on your mind."
        }),
        (@"how are you|how is it going|how's it going", new[]
        {
            "I'm running smoothly and ready to help you stay cyber-safe today!",
            "All systems green, thanks for asking! What shall we look at?",
            "Doing great, {name}. Hackers aren't getting past me today!"
        }),
        (@"\b(thanks|thank you|thx|cheers)\b", new[]
        {
            "You're welcome, {name}! Want another tip?",
            "Happy to help! Staying curious is the best defence.",
            "Anytime! Ask me about another topic whenever you like."
        }),
        (@"purpose|what do you do|why are you here", new[]
        {
            "My purpose is to educate South Africans about cyber threats like phishing, scams and malware so you can protect yourself online.",
            "I'm a Cybersecurity Awareness Assistant. I explain online threats in plain language and show you how to avoid them."
        }),
        (@"what can i ask|\bhelp\b|topics|what do you know", new[]
        {
            "You can ask me about passwords, phishing, scams, privacy, malware, safe browsing, social engineering, public Wi-Fi, identity theft, online banking and software updates. Say 'tell me more' to go deeper on any of them.",
            "I cover 11 topics, from passwords and phishing to banking safety and Wi-Fi. Name one, or ask for 'an example' once we're talking about it."
        }),
        (@"your name|who are you", new[]
        {
            "I'm the Cybersecurity Awareness Bot. You can just call me Bot!",
            "They call me the Cybersecurity Awareness Bot, but Bot works fine."
        }),
        (@"who (made|created|built) you", new[]
        {
            "I was built as a student project to help South Africans stay safe online.",
            "A software development student built me to make cybersecurity easier to understand."
        }),
        (@"\bjoke\b", new[]
        {
            "Why was the computer cold? It left its Windows open!",
            "I'd tell you a UDP joke, but you might not get it.",
            "My password is 'incorrect', so whenever I forget it the computer reminds me: 'Your password is incorrect.'"
        }),
        (@"^(ok|okay|cool|nice|great|alright|got it)\b", new[]
        {
            "Glad that helps! Want another tip, or shall we try a different topic?",
            "Great! Ask for 'another tip' or 'an example' whenever you're ready."
        })
    };
}