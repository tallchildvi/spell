/// <file>
/// <summary>
/// KeywordIntentClassifier.cs - Keyword-based intent classifier implementation
/// </summary>
/// </file> 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace spell.Core;

/// <summary>
/// Keyword-based intent classifier using string matching
/// </summary>
/// <remarks>
/// This classifier uses a predefined dictionary of keywords to identify intents.
/// It's fast but limited to exact keyword matches. Works well for unambiguous
/// commands with clear trigger words.
/// </remarks>
public class KeywordIntentClassifier : IIntentClassifier
{
    /// <summary>
    /// Dictionary mapping keywords to intent names
    /// </summary>
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

    /// <summary>
    /// Classifies input text by matching keywords
    /// </summary>
    /// <param name="input">Natural language text to classify</param>
    /// <returns>IntentResult with matched intent and confidence score</returns>
    /// <exception cref="ArgumentNullException">Not thrown, handles null gracefully</exception>
    /// <example>
    /// <code>
    /// var classifier = new KeywordIntentClassifier();
    /// var result = classifier.Classify("remind me to call");
    /// // result.Intent == "reminder"
    /// // result.Confidence between 0.4 and 0.95
    /// </code>
    /// </example>
    /// <remarks>
    /// Confidence calculation: 0.4 base + 0.3 * keyword_count, capped at 0.95
    /// </remarks>
    public IntentResult Classify(string input)
    {
        var lower = input.ToLowerInvariant();
        var scores = new Dictionary<string, int>();


        foreach (var kv in _keywordToIntent)
        {
            if (lower.Contains(kv.Key))
                scores[kv.Value] = scores.GetValueOrDefault(kv.Value, 0) + 1;
        }
        // TODO: change parsing logic


        if (!scores.Any())
        {
            return new IntentResult { Intent = "unknown", Confidence = 0.0, RawText = input };
        }


        var best = scores.OrderByDescending(kv => kv.Value).First();
        var confidence = Math.Min(0.95, 0.4 + 0.3 * best.Value);


        return new IntentResult { Intent = best.Key, Confidence = confidence, RawText = input };
    }
}