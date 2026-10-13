using CybersecurityBot.Wpf.Models;
using System.Text.RegularExpressions;

namespace CybersecurityBot.Wpf.Services;

/// <summary>A handler returns a reply, or null if it doesn't apply to the input.</summary>
public delegate string? ResponseHandler(string text);

public class ResponseEngine
{
    private const string Fallback =
        "I'm not sure I understand. Can you try rephrasing? You can ask me about passwords, phishing, scams, privacy, malware or safe browsing.";

    private static readonly string[] MoreTipPhrases =
        { "another tip", "give me another", "one more", "next tip", "another one" };

    private static readonly string[] ExplainPhrases =
        { "tell me more", "explain", "more detail", "more info", "elaborate", "what do you mean",
          "i don't understand", "i dont understand", "confused" };

    private readonly User _user;
    private readonly Random _random = new();
    private readonly Dictionary<string, string> _lastTip = new();
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

        return Fallback;
    }

    // ---------- Handlers ----------
    private string? HandleFollowUp(string text)
    {
        if (FindTopic(text) != null) return null;   // a new topic was named, so let the topic handler answer

        bool wantsTip = MoreTipPhrases.Any(p => text.Contains(p));
        bool wantsDetail = ExplainPhrases.Any(p => text.Contains(p));
        if (!wantsTip && !wantsDetail) return null;

        if (string.IsNullOrEmpty(_user.LastTopic))
            return "We haven't picked a topic yet. Ask me about passwords, phishing, scams, privacy, malware or safe browsing and I'll go from there.";

        return wantsTip ? NextTip(_user.LastTopic) : KnowledgeBase.Details[_user.LastTopic];
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
        return $"Great! I'll remember that you're interested in {topic}. It's a crucial part of staying safe online. {NextTip(topic)}";
    }

    private string? HandleTopicOrSentiment(string text)
    {
        string? topic = FindTopic(text);
        string? sentiment = FindSentiment(text);
        if (topic == null && sentiment == null) return null;

        // Feeling but no topic: continue the last topic (or pick one) so the user gets a tip straight away
        topic ??= string.IsNullOrEmpty(_user.LastTopic) ? RandomTopic() : _user.LastTopic;
        _user.LastTopic = topic;

        string opener = sentiment != null ? KnowledgeBase.SentimentReplies[sentiment] + " " : string.Empty;
        return opener + NextTip(topic) + PersonalTouch(topic);
    }

    private string? FindSentiment(string text)
    {
        foreach (var pair in KnowledgeBase.SentimentWords)
            if (pair.Value.Any(word => text.Contains(word))) return pair.Key;
        return null;
    }

    private string RandomTopic()
    {
        List<string> topics = KnowledgeBase.Tips.Keys.ToList();
        return topics[_random.Next(topics.Count)];
    }

    private string PersonalTouch(string topic)
    {
        if (!string.Equals(_user.FavouriteTopic, topic, StringComparison.OrdinalIgnoreCase)) return string.Empty;
        return $" As someone interested in {topic}, you might want to {KnowledgeBase.PersonalNotes[topic]}";
    }

    private string? HandleSmallTalk(string text)
    {
        if (text.Contains("how are you"))
            return "I'm running smoothly and ready to help you stay cyber-safe today!";
        if (text.Contains("purpose") || text.Contains("what do you do"))
            return "My purpose is to educate South African citizens about cyber threats like phishing, scams and malware so you can protect yourself online.";
        if (text.Contains("what can i ask") || text.Contains("help") || text.Contains("topics"))
            return "You can ask me about: password safety, phishing, scams, privacy, malware and safe browsing.";
        if (text.Contains("your name"))
            return "I'm the Cybersecurity Awareness Bot. You can just call me Bot!";
        return null;
    }

    // ---------- Helpers ----------
    private string? FindTopic(string text)
    {
        foreach (var pair in KnowledgeBase.Keywords)
            if (text.Contains(pair.Key)) return pair.Value;
        return null;
    }

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
}