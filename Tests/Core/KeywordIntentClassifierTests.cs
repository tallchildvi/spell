using NUnit.Framework;
using spell.Core;

namespace Tests.Core;

[TestFixture]
public class KeywordIntentClassifierTests
{
    private KeywordIntentClassifier _classifier;

    [SetUp]
    public void SetUp()
    {
        _classifier = new KeywordIntentClassifier();
    }

    [Test]
    public void Classify_TextWithRemindKeyword_ReturnsReminderIntent()
    {
        string input = "remind me to call mom";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.EqualTo(0.7));
    }

    [Test]
    public void Classify_TextWithNoteKeyword_ReturnsNoteIntent()
    {
        string input = "take note of this important information";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("note"));
        Assert.That(result.Confidence, Is.EqualTo(0.7));
    }

    [Test]
    public void Classify_TextWithTimerKeyword_ReturnsTimerIntent()
    {
        string input = "set timer for 10 minutes";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("timer"));
        Assert.That(result.Confidence, Is.EqualTo(0.95));
    }

    [Test]
    public void Classify_TextWithConvertKeyword_ReturnsConvertIntent()
    {
        string input = "convert 5 kilometers to meters";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("convert"));
        Assert.That(result.Confidence, Is.EqualTo(0.95));
    }`

    [Test]
    public void Classify_TextWithoutKeywords_ReturnsUnknownIntent()
    {
        string input = "no key words in there";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
        Assert.That(result.Confidence, Is.EqualTo(0.0));
    }

    [Test]
    public void Classify_EmptyString_ReturnsUnknownIntent()
    {
        string input = string.Empty;

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
        Assert.That(result.Confidence, Is.EqualTo(0.0));
    }

    [Test]
    public void Classify_KeywordInUpperCase_RecognizesIntent()
    {
        string input = "REMIND me to call mom tomorrow";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.EqualTo(0.7));
    }

    [Test]
    public void Classify_KeywordInMixedCase_RecognizesIntent()
    {
        string input = "ReMiNd me about the meeting";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.EqualTo(0.7));
    }

    [Test]
    public void Classify_KeywordWithPunctuation_RecognizesIntent()
    {
        string input = "Please,remind me about this!";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.EqualTo(0.7));
    }

    [Test]
    public void Classify_TextWithMultipleKeywords_ReturnsFirstMatchedIntent()
    {
        string input = "remind me to take note of this";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("reminder"));
        Assert.That(result.Confidence, Is.EqualTo(0.7));
    }

    [Test]
    public void Classify_PartialKeywordMatch_DoesNotTriggerIntent()
    {
        string input = "this is a remainder of the calculation";

        var result = _classifier.Classify(input);

        Assert.That(result.Intent, Is.EqualTo("unknown"));
        Assert.That(result.Confidence, Is.EqualTo(0.0));
    }
}
