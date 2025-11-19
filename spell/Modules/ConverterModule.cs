/// <file>
/// <summary>
/// ConverterModule.cs - Module for unit conversion and measurement transformations
/// </summary>
/// </file>
using System;
using System.Collections.Generic;
using spell.Core;
using Microsoft.Recognizers.Text.NumberWithUnit;


namespace spell.Modules;

/// <summary>
/// Module for processing unit conversion requests
/// </summary>
/// <remarks>
/// Extracts and displays dimensional units from natural language.
/// Handles conversions like "5 kilograms to grams", "10 USD to EUR", 
/// "37 celsius to fahrenheit".
/// </remarks>


public class ConverterModule : IModule
{
    /// <summary>
    /// Processes conversion intent by extracting and displaying unit information
    /// </summary>
    /// <param name="command">IntentResult containing conversion request and extracted units</param>
    /// <exception cref="Exception">May throw during entity access or console output</exception>
    /// <example>
    /// <code>
    /// var result = new IntentResult
    /// {
    ///     Intent = "convert",
    ///     Confidence = 0.92,
    ///     Entities = new Dictionary&lt;string, object&gt;
    ///     {
    ///         ["units"] = recognizedUnits,
    ///         ["text"] = "convert 5 kg to grams"
    ///     }
    /// };
    /// ConverterModule.Process(result);
    /// // Output: Unit result: type=Dimension text='5 kg' resolution=...
    /// </code>
    /// </example>
    /// <remarks>
    /// Displays:
    /// - Intent confidence score
    /// - Recognized units with type, text, and resolution details
    /// - Raw conversion request if no units extracted
    /// </remarks>
    public static void Process(IntentResult command)
    {
        Console.WriteLine($"[Converter] Intent confidence: {command.Confidence}");


        if (command.Entities.TryGetValue("units", out var unitsObj) && unitsObj is IList<Microsoft.Recognizers.Text.ModelResult> units && units.Count > 0)
        {
            foreach (var u in units)
            {
                Console.WriteLine($"Unit result: type={u.TypeName} text='{u.Text}' resolution={u.Resolution}");
            }
        }
        else
        {
            Console.WriteLine($"Conversion request (raw): {command.Entities.GetValueOrDefault("text")}");
        }
    }

    /// <summary>
    /// Explicit interface implementation that delegates to static Process method
    /// </summary>
    /// <param name="command">IntentResult to process</param>
    void IModule.Process(IntentResult command) => Process(command);
}
