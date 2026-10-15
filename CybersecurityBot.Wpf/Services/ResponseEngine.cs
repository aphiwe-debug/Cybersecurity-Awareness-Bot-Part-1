using System.Text.RegularExpressions;
using CybersecurityBot.Wpf.Models;

namespace CybersecurityBot.Wpf.Services;

/// <summary>A handler returns a reply, or null if it doesn't apply to the input.</summary>
public delegate string? ResponseHandler(string text);

public class ResponseEngine
{
    private static readonly string[] MoreTipPhrases =
        { "another tip", "give me another", "one more", "next tip", "another one", "something else" };

    private static readonly string[] ExplainPhrases =
        { "tell me more", "explain", "more detail", "more info", "elaborate", "what do you mean",
          "i don't understand", "i dont understand", "confused", "go deeper" };

    private static readonly string[] ExamplePhrases = { "example", "for instance", "real life", "real-life" };

    private static readonly Regex InterestPattern = new(
    @"\b(?:interested in|care about|passionate about|love learning about)\s+(?<subject>.+)",
    RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // only count as "explain this topic" when a topic is also named, e.g. "what is phishing"
    private static readonly string[] DefinePhrases = { "what is", "what are", "what's", "how does", "how do" };

    private readonly User _user;
    private readonly Random _random = new();
    private readonly Dictionary<string, string> _lastTip = new();
    private readonly Dictionary<string, int> _depth = new();   // topic -> how far the "tell me more" ladder has gone
    private readonly List<ResponseHandler> _handlers;

    public ResponseEngine(User user)
    {
        _user = user;
        // Pipeline of delegates: the first handler that returns a reply wins
        _handlers = new List<ResponseHandler>
        {
           HandleRecall,
           HandleInterest,
           HandleFollowUp,
           HandleTopicOrSentiment,  
           HandleSmallTalk
        };
    }

    public string GetResponse(string input)
    {
        _user.QuestionsAsked++;
        string text = input.Trim().ToLowerInvariant();

        try
        {
            foreach (ResponseHandler handler in _handlers)
            {
                string? reply = handler(text);
                if (reply != null) return reply;
            }
        }
        catch (Exception)
        {
            return "Oops, something went wrong on my side. Please try asking again.";
        }

        return $"{Pick(KnowledgeBase.Fallbacks)} Try asking about {string.Join(", ", RandomTopics(3))}.";
    }

    // ---------- Handlers ----------
    private string? HandleFollowUp(string text)
    {
        string? named = FindTopic(text);

        bool wantsTip = MoreTipPhrases.Any(p => text.Contains(p));
        bool wantsExample = ExamplePhrases.Any(p => text.Contains(p));
        bool wantsDetail = ExplainPhrases.Any(p => text.Contains(p))
                           || (named != null && DefinePhrases.Any(p => text.Contains(p)));

        if (!wantsTip && !wantsExample && !wantsDetail) return null;

        string? topic = named ?? _user.LastTopic;
        if (string.IsNullOrEmpty(topic))
            return $"We haven't picked a topic yet. Try asking about {string.Join(", ", RandomTopics(3))}.";

        _user.LastTopic = topic;

        if (wantsExample)
        {
            _depth[topic] = Math.Max(_depth.GetValueOrDefault(topic), 2);
            return KnowledgeBase.Examples[topic];
        }

        if (wantsDetail) return Deepen(topic);
        return TipReply(topic);
    }

    private string? HandleRecall(string text)
    {
        bool asksWhatIKnow = text.Contains("what do you remember") || text.Contains("what's my name")
                          || text.Contains("what is my name") || text.Contains("do you remember me");
        if (!asksWhatIKnow) return null;

        string favourite = string.IsNullOrEmpty(_user.FavouriteTopic)
            ? "You haven't told me a favourite topic yet."
            : $"You're interested in {_user.FavouriteTopic}.";
        return $"Your name is {_user.Name}. {favourite} You've asked me {Math.Max(0, _user.QuestionsAsked - 1)} question(s) so far.";
    }

    private string? HandleInterest(string text)
    {
        Match match = InterestPattern.Match(text);
        if (!match.Success) return null;

        string? topic = FindTopic(match.Groups["subject"].Value);
        if (topic == null) return null;

        _user.FavouriteTopic = topic;   // memory: used later by PersonalTouch
        _user.LastTopic = topic;
        _depth[topic] = 0;
        return $"Great! I'll remember that you're interested in {topic}. It's a crucial part of staying safe online. {NextTip(topic)}";
    }

    private string? HandleTopicOrSentiment(string text)
    {
        string? topic = FindTopic(text);
        string? sentiment = FindSentiment(text);
        if (topic == null && sentiment == null) return null;

        // Feeling but no topic: continue the last topic (or pick one) so the user gets a tip straight away
        topic ??= string.IsNullOrEmpty(_user.LastTopic) ? RandomTopics(1)[0] : _user.LastTopic;
        _user.LastTopic = topic;
        _depth[topic] = 0;

        // The sentiment opener replaces the random intro instead of stacking on it
        string opener = sentiment != null ? KnowledgeBase.SentimentReplies[sentiment] + " " : string.Empty;
        string tip = sentiment == null ? TipReply(topic) : NextTip(topic);
        return opener + tip + PersonalTouch(topic);
    }

    private string? FindSentiment(string text)
    {
        foreach (var pair in KnowledgeBase.SentimentWords)
            if (pair.Value.Any(word => text.Contains(word))) return pair.Key;
        return null;
    }

    private string PersonalTouch(string topic)
    {
        if (!string.Equals(_user.FavouriteTopic, topic, StringComparison.OrdinalIgnoreCase)) return string.Empty;
        return $" As someone interested in {topic}, you might want to {KnowledgeBase.PersonalNotes[topic]}";
    }

    private string? HandleSmallTalk(string text)
    {
        foreach (var (pattern, replies) in KnowledgeBase.SmallTalk)
        {
            if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
                return Pick(replies).Replace("{name}", _user.Name);
        }
        return null;
    }

    // ---------- Helpers ----------
    private string? FindTopic(string text)
    {
        foreach (var pair in KnowledgeBase.Keywords)
            if (text.Contains(pair.Key)) return pair.Value;
        return null;
    }

    /// <summary>Tip wrapped in a randomly chosen intro so replies don't all sound the same.</summary>
    private string TipReply(string topic) => $"{Pick(KnowledgeBase.TipIntros)} {NextTip(topic)}";

    /// <summary>Random tip for the topic, never the same one twice in a row.</summary>
    private string NextTip(string topic)
    {
        List<string> tips = KnowledgeBase.Tips[topic];
        string tip;
        do
        {
            tip = tips[_random.Next(tips.Count)];
        }
        while (tips.Count > 1 && _lastTip.TryGetValue(topic, out string? last) && tip == last);

        _lastTip[topic] = tip;
        return tip;
    }

    /// <summary>Depth ladder: explanation, then real-world example, then a nudge to a new topic.</summary>
    private string Deepen(string topic)
    {
        int level = _depth.GetValueOrDefault(topic);
        _depth[topic] = level + 1;

        return level switch
        {
            0 => KnowledgeBase.Details[topic],
            1 => KnowledgeBase.Examples[topic],
            _ => $"That's everything I have on {topic} for now. Ask for another tip, or try a different topic such as {RandomOtherTopic(topic)}."
        };
    }

    private string Pick(IReadOnlyList<string> items) => items[_random.Next(items.Count)];

    private List<string> RandomTopics(int count) =>
        KnowledgeBase.Tips.Keys.OrderBy(_ => _random.Next()).Take(count).ToList();

    private string RandomOtherTopic(string current) =>
        KnowledgeBase.Tips.Keys.Where(t => t != current).OrderBy(_ => _random.Next()).First();

    // ---------- Sentiment ----------//
    public static readonly Dictionary<string, string[]> SentimentWords = new()
    {
        ["worried"] = new[] { "worried", "scared", "afraid", "anxious", "nervous", "concerned" },
        ["frustrated"] = new[] { "frustrated", "annoyed", "angry", "fed up", "hate" },
        ["overwhelmed"] = new[] { "overwhelmed", "too much", "lost" },
        ["curious"] = new[] { "curious", "wondering", "want to learn" }
    };

    public static readonly Dictionary<string, string> SentimentReplies = new()
    {
        ["worried"] = "It's completely understandable to feel that way. Cyber threats can be very convincing, but a few habits go a long way. Here's a tip:",
        ["frustrated"] = "I hear you, security can feel like a hassle sometimes. Let's keep it simple. Here's one quick tip:",
        ["overwhelmed"] = "No need to feel overwhelmed. We'll take it one small step at a time. Start with this:",
        ["curious"] = "Great that you're curious, asking questions is the best defence. Here's something to know:"
    };

    // completes "As someone interested in X, you might want to ..." (one entry per topic)
    public static readonly Dictionary<string, string> PersonalNotes = new()
    {
        ["password"] = "review which of your accounts still share a password and change those first.",
        ["phishing"] = "pause before clicking links in any message that creates a sense of urgency.",
        ["scam"] = "double-check any unexpected request for money or a one-time PIN before you act.",
        ["privacy"] = "review the security settings on your accounts.",
        ["malware"] = "check that your device updates and antivirus are switched on.",
        ["browsing"] = "make it a habit to check the address bar before entering login details.",
        ["social engineering"] = "agree a family code word and always call back on an official number before acting on an urgent request.",
        ["wifi"] = "switch off auto-connect to open networks on your phone and laptop.",
        ["identity"] = "request your free credit report and check it for accounts you don't recognise.",
        ["banking"] = "turn on transaction notifications in your banking app today.",
        ["updates"] = "switch on automatic updates and check when your last backup was."
    };
}