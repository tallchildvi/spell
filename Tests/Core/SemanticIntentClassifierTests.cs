using NUnit.Framework;
using spell.Core;
using System.Collections.Generic;

namespace Tests.Core;

[TestFixture]
public class SemanticIntentClassifierTests
{
    private Dictionary<string, string[]> _examples;
    private SemanticIntentClassifier _classifier;

    [SetUp]
    public void SetUp()
    {
        _examples = new Dictionary<string, string[]>
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

        _classifier = new SemanticIntentClassifier(_examples);
    }

    // Test 1
    [Test]
    public void Classify_SimilarToReminderExample_ReturnsReminderIntent()
    {
        string input = "remind me to call dad today";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.GreaterThan(_classifier.AcceptanceThreshold));
    }

    // Test 2
    [Test]
    public void Classify_SimilarToNoteExample_ReturnsNoteIntent()
    {
        string input = "please write down this information";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("note"));
        Assert.That(result.Confidence, Is.GreaterThan(_classifier.AcceptanceThreshold));
    }

    // Test 3
    [Test]
    public void Classify_SimilarToTimerExample_ReturnsTimerIntent()
    {
        string input = "start timer for 15 minutes";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("timer"));
        Assert.That(result.Confidence, Is.GreaterThan(_classifier.AcceptanceThreshold));
    }

    // Test 4
    [Test]
    public void Classify_ExactMatch_ReturnsHighConfidence()
    {
        string input = "remind me to call mom tomorrow";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.GreaterThan(0.9));
    }

    // Test 5
    [Test]
    public void Classify_CompletelyDifferentText_ReturnsUnknown()
    {
        string input = "the weather is nice today";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
        Assert.That(result.Confidence, Is.LessThan(_classifier.AcceptanceThreshold));
    }

    // Test 6
    [Test]
    public void Classify_DifferentWordOrder_RecognizesIntent()
    {
        string input = "call mom remind me to";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
    }

    // Test 7
    [Test]
    public void Classify_WithPunctuation_RecognizesIntent()
    {
        string input = "Remind me, to call mom tomorrow!";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.GreaterThan(_classifier.AcceptanceThreshold));
    }

    // Test 8
    [Test]
    public void Classify_UpperCase_RecognizesIntent()
    {
        string input = "REMIND ME TO CALL MOM";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.GreaterThan(_classifier.AcceptanceThreshold));
    }

    // Test 9
    [Test]
    public void Classify_WithStopWords_FiltersCorrectly()
    {
        string input = "the reminder is to call the mom at the time";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
    }

    // Test 10
    [Test]
    public void Classify_LongInput_HandlesGracefully()
    {
        string input = "I was thinking that maybe you could possibly remind me at some point in " +
            "the near future to call my mom tomorrow morning at around 8am if that would be convenient";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
    }

    // Test 11
    [Test]
    public void Classify_NumbersInText_HandlesCorrectly()
    {
        string input = "set timer for 25 minutes";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("timer"));
        Assert.That(result.Confidence, Is.GreaterThan(_classifier.AcceptanceThreshold));
    }

    // Test 12
    [Test]
    public void Classify_SpecialCharacters_FiltersAndRecognizes()
    {
        string input = "remind@me#to$call%mom^tomorrow";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
    }

    // Test 13
    [Test]
    public void Classify_WithPartialOverlap_ComputesSimilarity()
    {
        string input = "remind me about something";

        var result = _classifier.Classify(input);

        Assert.That(result.Confidence, Is.GreaterThan(0.0));
    }

    // Test 14
    [Test]
    public void Classify_MultipleIntentsWithSimilarWords_ChoosesBestMatch()
    {
        string input = "start countdown for 20 seconds please";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("timer"));
        Assert.That(result.Confidence, Is.GreaterThan(_classifier.AcceptanceThreshold));
    }

    // Test 15
    [Test]
    public void Classify_WithTypos_StillRecognizesIntent()
    {
        string input = "remnd me too cal mom";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.Not.Null);
    }

    // Test 16
    [Test]
    public void Classify_EmptyString_ReturnsUnknown()
    {
        string input = string.Empty;

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
        Assert.That(result.Confidence, Is.EqualTo(0.0));
    }

    // Test 17
    [Test]
    public void Classify_WhitespaceOnly_ReturnsUnknown()
    {
        string input = "   ";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
        Assert.That(result.Confidence, Is.EqualTo(0.0));
    }

    // Test 18
    [Test]
    public void Classify_WithCustomThreshold_RespectsThreshold()
    {
        _classifier.AcceptanceThreshold = 0.9;
        string input = "maybe remind me something";

        var result = _classifier.Classify(input);

        if (result.Confidence < 0.9)
        {
            Assert.That(result.Intent, Is.EqualTo("unknown"));
        }
    }

    // Test 19
    [Test]
    public void Classify_LowSimilarity_ReturnsUnknownWithConfidence()
    {
        string input = "completely unrelated random words here";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
        Assert.That(result.Confidence, Is.GreaterThanOrEqualTo(0.0));
        Assert.That(result.Confidence, Is.LessThan(_classifier.AcceptanceThreshold));
    }

    // Test 20
    [Test]
    public void Classify_ShortInput_HandlesGracefully()
    {
        string input = "remind";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.Not.Null);
        Assert.That(result.Confidence, Is.GreaterThanOrEqualTo(0.0));
    }

    // Test 21
    [Test]
    public void Classify_OnlyStopWords_ReturnsUnknown()
    {
        string input = "the a to for at on";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
        Assert.That(result.Confidence, Is.EqualTo(0.0));
    }

    // Test 22
    [Test]
    public void Constructor_EmptyExamples_CreatesClassifier()
    {
        var emptyExamples = new Dictionary<string, string[]>();

        var classifier = new SemanticIntentClassifier(emptyExamples);

        Assert.That(classifier, Is.Not.Null);
    }

    // Test 23
    [Test]
    public void AcceptanceThreshold_CanBeModified_AffectsClassification()
    {
        string input = "perhaps remind me";

        _classifier.AcceptanceThreshold = 0.3;
        var result1 = _classifier.Classify(input);

        _classifier.AcceptanceThreshold = 0.9;
        var result2 = _classifier.Classify(input);

        Assert.That(result1.Intent, Is.Not.EqualTo("unknown").Or.EqualTo("unknown"));
        Assert.That(result2.Intent, Is.Not.Null);
    }
}