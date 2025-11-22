/// <file>
/// <summary>
/// HybridClassifier.cs - Combines multiple classifiers with fallback logic
/// </summary>
/// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Core;

/// <summary>
/// Hybrid classifier combining primary and fallback classification strategies
/// </summary>
/// <remarks>
/// Uses primary classifier first (fast), falls back to secondary classifier
/// if primary confidence is low or returns unknown intent
/// </remarks>
/// 
public class HybridClassifier : IIntentClassifier
{
    /// <summary>
    /// Primary classifier (typically keyword-based for speed)
    /// </summary>
    private readonly IIntentClassifier _primary;

    /// <summary>
    /// Fallback classifier (typically semantic for accuracy)
    /// </summary>
    private readonly IIntentClassifier _fallback;

    /// <summary>
    /// Gets or sets the confidence threshold for primary classifier acceptance
    /// </summary>
    /// <value>Threshold value between 0.0 and 1.0, defaults to 0.5</value>
    /// <remarks>
    /// If primary confidence is below this threshold, fallback classifier is used
    /// </remarks>

    public double PrimaryConfidenceThreshold { get; set; } = 0.5;

    /// <summary>
    /// Initializes a new instance of HybridClassifier
    /// </summary>
    /// <param name="primary">Primary classifier to try first</param>
    /// <param name="fallback">Fallback classifier to use if primary fails</param>
    /// <exception cref="ArgumentNullException">Thrown when primary or fallback is null</exception>
    /// <example>
    /// <code>
    /// var keyword = new KeywordIntentClassifier();
    /// var semantic = new SemanticIntentClassifier(examples);
    /// var hybrid = new HybridClassifier(keyword, semantic)
    /// {
    ///     PrimaryConfidenceThreshold = 0.6
    /// };
    /// </code>
    /// </example>

    public HybridClassifier(IIntentClassifier primary, IIntentClassifier fallback)
    {
        _primary = primary ?? throw new ArgumentNullException(nameof(primary));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
    }

    /// <summary>
    /// Classifies input using hybrid strategy with fallback logic
    /// </summary>
    /// <param name="input">Natural language text to classify</param>
    /// <returns>Best IntentResult from primary or fallback classifier</returns>
    /// <exception cref="Exception">May propagate exceptions from underlying classifiers</exception>
    /// <example>
    /// <code>
    /// var result = hybrid.Classify("remind me to call mom");
    /// // Uses keyword classifier first
    /// // Falls back to semantic if confidence &lt; threshold
    /// </code>
    /// </example>
    /// <remarks>
    /// Decision logic:
    /// 1. Try primary classifier
    /// 2. If result is unknown or confidence &lt; threshold, try fallback
    /// 3. Return fallback result if better than primary
    /// </remarks
    /// 
    public IntentResult Classify(string input)
    {
        var res = _primary.Classify(input);
        if (res == null) return _fallback.Classify(input);

        if (string.IsNullOrEmpty(res.Intent) || res.Intent == "unknown" || res.Confidence < PrimaryConfidenceThreshold)
        {
            var fb = _fallback.Classify(input);

            if (fb != null && fb.Intent != "unknown" && fb.Confidence > res.Confidence)
            {
                return fb;
            }
        }

        return res;
    }
}
