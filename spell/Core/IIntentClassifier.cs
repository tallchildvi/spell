/// <file>
/// <summary>
/// IIntentClassifier.cs - Interface for intent classification in NLP pipeline
/// </summary>
/// </file>
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Core;

/// <summary>
/// Interface for intent classification components in the NLP pipeline
/// </summary>
/// <remarks>
/// Implementations of this interface analyze natural language text and classify
/// it into predefined intent categories with confidence scores.
/// </remarks>

public interface IIntentClassifier
{
    IntentResult Classify(string input);
}