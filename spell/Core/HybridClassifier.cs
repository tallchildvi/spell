using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Core;

public class HybridClassifier : IIntentClassifier
{
    private readonly IIntentClassifier _primary;
    private readonly IIntentClassifier _fallback;

    public double PrimaryConfidenceThreshold { get; set; } = 0.5;

    public HybridClassifier(IIntentClassifier primary, IIntentClassifier fallback)
    {
        _primary = primary ?? throw new ArgumentNullException(nameof(primary));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
    }

    public IntentResult Classify(string input)
    {
        var res = _primary.Classify(input);
        if (res == null) return _fallback.Classify(input);

        if (string.IsNullOrEmpty(res.Intent) || res.Intent == "unknown" || res.Confidence < PrimaryConfidenceThreshold)
        {
            var fb = _fallback.Classify(input);

            if (fb != null && fb.Intent != "unknown" && fb.Confidence > res.Confidence)
            {
                return fb;
            }
        }

        return res;
    }
}
