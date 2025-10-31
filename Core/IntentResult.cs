using System.Collections.Generic;

namespace spell.Core;
public class IntentResult
{
    public string Intent { get; set; } = "unknown";
    public double Confidence { get; set; } = 0.0;
    public Dictionary<string, object> Entities { get; set; } = new();
    public string RawText { get; set; } = string.Empty;
}