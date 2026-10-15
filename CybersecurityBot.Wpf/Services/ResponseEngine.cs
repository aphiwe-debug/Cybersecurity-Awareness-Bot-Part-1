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
            HandleFollowUp,
            HandleTopic,
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

    private string? HandleTopic(string text)
    {
        string? topic = FindTopic(text);
        if (topic == null) return null;

        _user.LastTopic = topic;   // remembered so follow-ups continue this topic
        _depth[topic] = 0;         // a fresh question restarts the "tell me more" ladder
        return TipReply(topic);
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
}