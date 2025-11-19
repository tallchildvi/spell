/// <file>
/// <summary>
/// IEntityExtractor.cs - Interface for entity extraction from natural language text
/// </summary>
/// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Core;

/// <summary>
/// Interface for entity extraction components in the NLP pipeline
/// </summary>
/// <remarks>
/// Implementations extract structured entities (dates, numbers, units) from text
/// and populate them into the IntentResult's Entities dictionary.
/// </remarks>

public interface IEntityExtractor
{
    /// <summary>
    /// Extracts entities from input text and populates the IntentResult
    /// </summary>
    /// <param name="input">Natural language text to extract entities from</param>
    /// <param name="result">IntentResult to populate with extracted entities</param>
    /// <example>
    /// <code>
    /// var result = new IntentResult { RawText = "remind me in 5 minutes" };
    /// IEntityExtractor extractor = new RecognizersEntityExtractor();
    /// extractor.Extract("remind me in 5 minutes", result);
    /// // result.Entities["datetime"] contains parsed time information
    /// </code>
    /// </example>
    void Extract(string input, IntentResult result);
}
