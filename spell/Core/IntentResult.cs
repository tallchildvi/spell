/// <file>
/// <summary>
/// IntentResult.cs - Data structure for storing intent classification results
/// </summary>
/// </file>

using System.Collections.Generic;

namespace spell.Core;

/// <summary>
/// Represents the result of intent classification and entity extraction
/// </summary>
/// <remarks>
/// This class stores the classified intent, confidence score, extracted entities,
/// and the original raw text. It serves as the communication object between
/// NLP pipeline components and module processors.
/// </remarks>
/// 
public class IntentResult
{
    /// <summary>
    /// Gets or sets the classified intent name (e.g., "reminder", "note", "timer", "convert", "unknown")
    /// </summary>
    /// <value>Intent name as string, defaults to "unknown"</value>
    public string Intent { get; set; } = "unknown";

    /// <summary>
    /// Gets or sets the confidence score of the classification
    /// </summary>
    /// <value>Confidence value between 0.0 and 1.0, defaults to 0.0</value>
    public double Confidence { get; set; } = 0.0;
    /// <summary>
    /// Gets or sets the dictionary of extracted entities
    /// </summary>
    /// <value>Dictionary where key is entity type (e.g., "datetime", "number") and value is the extracted entity</value>
    /// <example>
    /// <code>
    /// var result = new IntentResult();
    /// result.Entities["datetime"] = parsedDateTime;
    /// result.Entities["text"] = "buy milk";
    /// </code>
    /// </example>
    public Dictionary<string, object> Entities { get; set; } = new();

    /// <summary>
    /// Gets or sets the original input text
    /// </summary>
    /// <value>Raw unprocessed input text, defaults to empty string</value>
    public string RawText { get; set; } = string.Empty;
}