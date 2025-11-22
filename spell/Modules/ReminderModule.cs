/// <file>
/// <summary>
/// ReminderModule.cs - Module for creating and managing reminders
/// </summary>
/// </file>
using Microsoft.Recognizers.Text;
using spell.Core;
using System;


namespace spell.Modules;

/// <summary>
/// Module for processing reminder creation requests
/// </summary>
/// <remarks>
/// Extracts datetime entities and reminder text from natural language.
/// Handles commands like "remind me to call mom tomorrow",
/// "set a reminder for 8am", "reminder to buy milk".
/// </remarks>

public class ReminderModule : IModule
{
    /// <summary>
    /// Processes reminder intent by extracting datetime and text information
    /// </summary>
    /// <param name="result">IntentResult containing reminder request and extracted entities</param>
    /// <exception cref="Exception">May throw during entity access or console output</exception>
    /// <example>
    /// <code>
    /// var result = new IntentResult
    /// {
    ///     Intent = "reminder",
    ///     Confidence = 0.88,
    ///     Entities = new Dictionary&lt;string, object&gt;
    ///     {
    ///         ["datetime"] = recognizedDateTime,
    ///         ["text"] = "call mom"
    ///     }
    /// };
    /// ReminderModule.Process(result);
    /// // Output: Reminder datetime text: tomorrow
    /// // Output: Parsed datetime: 2024-01-16T00:00:00
    /// // Output: Reminder text: call mom
    /// </code>
    /// </example>
    /// <remarks>
    /// Displays:
    /// - Datetime text as recognized in input (e.g., "tomorrow", "5 minutes")
    /// - Parsed datetime value(s) from resolution
    /// - Reminder text content
    /// </remarks>
    public static void Process(IntentResult result)
    {
        if (result.Entities.TryGetValue("datetime", out var dtObj) && dtObj is List<ModelResult> dtList)
        {
            foreach (var dt in dtList)
            {
                Console.WriteLine($"Reminder datetime text: {dt.Text}");

                if (dt.Resolution != null && dt.Resolution.TryGetValue("values", out var valuesObj) && valuesObj is List<Dictionary<string, string>> values)
                {
                    foreach (var val in values)
                    {
                        if (val.TryGetValue("value", out var datetimeValue))
                        {
                            Console.WriteLine($"Parsed datetime: {datetimeValue}");
                        }
                    }
                }
            }
        }

        if (result.Entities.TryGetValue("text", out var text))
        {
            Console.WriteLine($"Reminder text: {text}");
        }
    }

    /// <summary>
    /// Explicit interface implementation that delegates to static Process method
    /// </summary>
    /// <param name="command">IntentResult to process</param>
    void IModule.Process(IntentResult command) => Process(command);
}