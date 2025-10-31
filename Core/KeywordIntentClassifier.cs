using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace spell.Core;


public class KeywordIntentClassifier : IIntentClassifier
{
    private readonly Dictionary<string, string> _keywordToIntent = new()
    {
    {"remind", "reminder"},
    {"reminder", "reminder"},
    {"remember", "note"},
    {"note", "note"},
    {"take note", "note"},
    {"timer", "timer"},
    {"in ", "timer"},
    {"for ", "timer"},
    {"convert", "convert"},
    {"to ", "convert"},
    {"how many", "convert"}
    };
    // TODO: Extend dictionary


    public IntentResult Classify(string input)
    {
        var lower = input.ToLowerInvariant();
        var scores = new Dictionary<string, int>();


        foreach (var kv in _keywordToIntent)
        {
            if (lower.Contains(kv.Key))
                scores[kv.Value] = scores.GetValueOrDefault(kv.Value, 0) + 1;
        }


        if (!scores.Any())
        {
            return new IntentResult { Intent = "unknown", Confidence = 0.0, RawText = input };
        }


        var best = scores.OrderByDescending(kv => kv.Value).First();
        var confidence = Math.Min(0.95, 0.4 + 0.3 * best.Value);


        return new IntentResult { Intent = best.Key, Confidence = confidence, RawText = input };
    }
}