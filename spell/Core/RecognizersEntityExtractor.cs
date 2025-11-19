/// <file>
/// <summary>
/// RecognizersEntityExtractor.cs - Entity extractor using Microsoft.Recognizers.Text
/// </summary>
/// </file>
/// 
using System;
using System.Collections.Generic;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.NumberWithUnit;


namespace spell.Core;

/// <summary>
/// Entity extractor using Microsoft.Recognizers.Text library
/// </summary>
/// <remarks>
/// Extracts dates/times, numbers, and dimensional units from text.
/// Gracefully handles extraction failures and ensures text entity is always present.
/// </remarks>

public class RecognizersEntityExtractor : IEntityExtractor
{
    /// <summary>
    /// Culture setting for recognition (English)
    /// </summary>
    private readonly string _culture = Culture.English;

    /// <summary>
    /// Extracts entities from input text and populates IntentResult
    /// </summary>
    /// <param name="input">Natural language text to extract entities from</param>
    /// <param name="result">IntentResult to populate with extracted entities</param>
    /// <exception cref="Exception">Exceptions are caught and ignored to ensure robustness</exception>
    /// <example>
    /// <code>
    /// var extractor = new RecognizersEntityExtractor();
    /// var result = new IntentResult { RawText = "remind me in 5 minutes" };
    /// extractor.Extract("remind me in 5 minutes", result);
    /// // result.Entities["datetime"] contains parsed time
    /// // result.Entities["number"] contains 5
    /// // result.Entities["text"] contains original text
    /// </code>
    /// </example>
    /// <remarks>
    /// Extracts:
    /// - datetime: dates and times (e.g., "tomorrow", "5 minutes", "at 8am")
    /// - number: numeric values (e.g., "5", "twenty", "3.14")
    /// - units: dimensional units (e.g., "5 kilograms", "10 meters")
    /// - text: original input as fallback
    /// </remarks>
    /// 
    public void Extract(string input, IntentResult result)
    {
        try
        {
            var dt = DateTimeRecognizer.RecognizeDateTime(input, _culture);
            if (dt != null && dt.Count > 0)
            {
                result.Entities["datetime"] = dt;
            }
        }
        catch { }


        try
        {
            var nums = NumberRecognizer.RecognizeNumber(input, _culture);
            if (nums != null && nums.Count > 0)
            {
                result.Entities["number"] = nums;
            }
        }
        catch { }


        try
        {
            var recognizer = new NumberWithUnitRecognizer(_culture);
            var model = recognizer.GetDimensionModel(_culture);
            var units = model.Parse(input);

            if (units != null && units.Count > 0)
            {
                result.Entities["units"] = units;
            }
        }
        catch { }


        if (!result.Entities.ContainsKey("text") || result.Entities["text"] == null || string.IsNullOrWhiteSpace(result.Entities["text"].ToString()))
        {
            result.Entities["text"] = input;
        }

    }
}