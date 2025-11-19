/// <file>
/// <summary>
/// IModule.cs - Interface for command processing modules
/// </summary>
/// </file>
/// 
using spell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Modules;

/// <summary>
/// Interface for modules that process intent classification results
/// </summary>
/// <remarks>
/// Implementations handle specific intent types and execute corresponding actions.
/// Modules receive IntentResult containing intent, confidence, and extracted entities.
/// </remarks>


public interface IModule
{
    /// <summary>
    /// Processes the classified intent and executes corresponding module logic
    /// </summary>
    /// <param name="command">IntentResult containing classified intent and extracted entities</param>
    /// <exception cref="Exception">May throw exceptions during processing</exception>
    /// <example>
    /// <code>
    /// IModule module = new ReminderModule();
    /// var result = new IntentResult 
    /// { 
    ///     Intent = "reminder", 
    ///     Confidence = 0.95,
    ///     Entities = new Dictionary&lt;string, object&gt; { ["text"] = "call mom" }
    /// };
    /// module.Process(result);
    /// </code>
    /// </example>
    void Process(IntentResult command);
}