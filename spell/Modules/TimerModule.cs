/// <file>
/// <summary>
/// TimerModule.cs - Module for timer and countdown management
/// </summary>
/// </file>
using System;
using System.Collections.Generic;
using spell.Core;
using Microsoft.Recognizers.Text;


namespace spell.Modules;

/// <summary>
/// Module for processing timer and countdown requests
/// </summary>
/// <remarks>
/// Extracts duration entities from natural language.
/// Handles commands like "set timer for 10 minutes",
/// "start countdown for 30 seconds", "timer 5 minutes".
/// </remarks>

public class TimerModule : IModule
{
    /// <summary>
    /// Processes timer intent by extracting duration information
    /// </summary>
    /// <param name="command">IntentResult containing timer request and extracted entities</param>
    /// <exception cref="Exception">May throw during entity access or console output</exception>
    /// <example>
    /// <code>
    /// var result = new IntentResult
    /// {
    ///     Intent = "timer",
    ///     Confidence = 0.91,
    ///     Entities = new Dictionary&lt;string, object&gt;
    ///     {
    ///         ["datetime"] = recognizedDuration,
    ///         ["text"] = "set timer for 5 minutes"
    ///     }
    /// };
    /// TimerModule.Process(result);
    /// // Output: [Timer] Intent confidence: 0.91
    /// // Output: Found datetime/entity: type=datetimeV2.duration text='5 minutes'
    /// </code>
    /// </example>
    /// <remarks>
    /// Displays:
    /// - Intent confidence score
    /// - Recognized datetime/duration entities with type and text
    /// - Raw timer request if no entities extracted
    /// </remarks>
    public static void Process(IntentResult command)
    {
        Console.WriteLine($"[Timer] Intent confidence: {command.Confidence}");
        if (command.Entities.TryGetValue("datetime", out var dtObj) && dtObj is IList<Microsoft.Recognizers.Text.ModelResult> list && list.Count > 0)
        {
            foreach (var m in list)
            {
                Console.WriteLine($"Found datetime/entity: type={m.TypeName} text='{m.Text}'");
            }
        }
        else
        {
            Console.WriteLine($"Timer started (raw): {command.Entities.GetValueOrDefault("text")}");
        }
    }

    /// <summary>
    /// Explicit interface implementation that delegates to static Process method
    /// </summary>
    /// <param name="command">IntentResult to process</param>

    void IModule.Process(IntentResult command) => Process(command);
}