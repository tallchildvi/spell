/// <file>
/// <summary>
/// NlpPipeline.cs - Orchestrates intent classification and entity extraction
/// </summary>
/// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Core;

/// <summary>
/// NLP processing pipeline that chains classification and extraction
/// </summary>
/// <remarks>
/// Coordinates the two-step NLP process: first classifies intent,
/// then extracts entities from the input text
/// </remarks>

public class NlpPipeline
{
    /// <summary>
    /// Intent classifier component
    /// </summary>
    private IIntentClassifier _classifier;
    /// <summary>
    /// Entity extractor component
    /// </summary>
    private IEntityExtractor _extractor;

    /// <summary>
    /// Initializes a new instance of NlpPipeline
    /// </summary>
    /// <param name="classifier">Intent classifier to use for classification</param>
    /// <param name="extractor">Entity extractor to use for extraction</param>
    /// <exception cref="ArgumentNullException">Thrown when classifier or extractor is null</exception>
    /// <example>
    /// <code>
    /// var classifier = new HybridClassifier(keyword, semantic);
    /// var extractor = new RecognizersEntityExtractor();
    /// var pipeline = new NlpPipeline(classifier, extractor);
    /// </code>
    /// </example>
    public NlpPipeline(IIntentClassifier classifier, IEntityExtractor extractor)
    {
        _classifier = classifier;
        _extractor = extractor;
    }

    /// <summary>
    /// Processes input through the complete NLP pipeline
    /// </summary>
    /// <param name="input">Natural language command text</param>
    /// <returns>IntentResult containing intent, confidence, and extracted entities</returns>
    /// /// <example>
    /// <code>
    /// var pipeline = new NlpPipeline(classifier, extractor);
    /// var result = pipeline.Handle("remind me to call mom tomorrow at 5pm");
    /// // result.Intent == "reminder"
    /// // result.Entities["datetime"] contains parsed date/time
    /// // result.Entities["text"] contains the reminder text
    /// </code>
    /// </example>
    /// <remarks>
    /// Processing steps:
    /// 1. Classify intent using configured classifier
    /// 2. Extract entities using configured extractor
    /// 3. Return populated IntentResult
    /// </remarks>

    public IntentResult Handle(string input)
    {
        var res = _classifier.Classify(input);
        _extractor.Extract(input, res);
        return res;
    }
}