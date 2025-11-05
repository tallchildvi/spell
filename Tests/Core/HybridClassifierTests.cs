using NUnit.Framework;
using spell.Core;
using System.Collections.Generic;

namespace Tests.Core;

[TestFixture]
public class HybridClassifierTests
{
    private KeywordIntentClassifier _primary;
    private SemanticIntentClassifier _fallback;
    private HybridClassifier _classifier;

    [SetUp]
    public void SetUp()
    {
        var examples = new Dictionary<string, string[]>
        {
            ["reminder"] = new[]
            {
                "remind me to call mom tomorrow",
                "set a reminder for 8am",
                "reminder to buy milk",
                "remind me about the meeting at 9"
            },
            ["note"] = new[]
            {
                "take note of this",
                "remember that I have a meeting",
                "save this note",
                "write this down"
            },
            ["timer"] = new[]
            {
                "set timer for 10 minutes",
                "start countdown for 30 seconds",
                "run timer for 5 minutes"
            }
        };

        _primary = new KeywordIntentClassifier();
        _fallback = new SemanticIntentClassifier(examples);
        _classifier = new HybridClassifier(_primary, _fallback);
    }

    // Test 1
    [Test]
    public void Classify_KeywordMatch_ReturnsPrimaryResult()
    {
        string input = "reminder to call dad";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.GreaterThan(_classifier.PrimaryConfidenceThreshold));
    }

    // Test 2
    [Test]
    public void Classify_NoKeywordButSemanticMatch_ReturnsFallbackResult()
    {
        string input = "please write this information down";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("note"));
        Assert.That(result.Confidence, Is.GreaterThan(0.0));
    }


    // Test 3
    [Test]
    public void Classify_CompletelyUnknown_ReturnsUnknown()
    {
        string input = "the weather is nice today";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
    }

    // Test 4
    [Test]
    public void Classify_EmptyInput_HandlesGracefully()
    {
        string input = string.Empty;

        var result = _classifier.Classify(input);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Intent, Is.EqualTo("unknown"));
    }

}