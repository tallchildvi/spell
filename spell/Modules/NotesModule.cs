/// <file>
/// <summary>
/// NotesModule.cs - Module for note-taking and text storage
/// </summary>
/// </file>

using System;
using spell.Core;


namespace spell.Modules;

/// <summary>
/// Module for processing note creation requests
/// </summary>
/// <remarks>
/// Extracts and displays note text from natural language.
/// Handles commands like "take note of this", "remember that I have a meeting",
/// "note: buy coffee", "write this down".
/// </remarks>

public class NotesModule : IModule
{
    /// <summary>
    /// Processes note intent by extracting and displaying note text
    /// </summary>
    /// <param name="command">IntentResult containing note request and text</param>
    /// <exception cref="Exception">May throw during entity access or console output</exception>
    /// <example>
    /// <code>
    /// var result = new IntentResult
    /// {
    ///     Intent = "note",
    ///     Confidence = 0.87,
    ///     Entities = new Dictionary&lt;string, object&gt;
    ///     {
    ///         ["text"] = "buy coffee and milk"
    ///     },
    ///     RawText = "note: buy coffee and milk"
    /// };
    /// NotesModule.Process(result);
    /// // Output: [Note] Intent confidence: 0.87
    /// // Output: Note saved: buy coffee and milk
    /// </code>
    /// </example>
    /// <remarks>
    /// Displays:
    /// - Intent confidence score
    /// - Note text (from entities if available, otherwise uses RawText)
    /// Falls back to RawText if no text entity is present
    /// </remarks>
    public static void Process(IntentResult command)
    {
        Console.WriteLine($"[Note] Intent confidence: {command.Confidence}");
        var text = command.Entities.GetValueOrDefault("text")?.ToString() ?? command.RawText;
        Console.WriteLine($"Note saved: {text}");
    }

    /// <summary>
    /// Explicit interface implementation that delegates to static Process method
    /// </summary>
    /// <param name="command">IntentResult to process</param>

    void IModule.Process(IntentResult command) => Process(command);
}