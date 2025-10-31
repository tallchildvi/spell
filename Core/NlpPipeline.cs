using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Core;


public class NlpPipeline
{
    private IIntentClassifier _classifier;
    private IEntityExtractor _extractor;

    public NlpPipeline(IIntentClassifier classifier, IEntityExtractor extractor)
    {
        _classifier = classifier;
        _extractor = extractor;
    }


    public IntentResult Handle(string input)
    {
        var res = _classifier.Classify(input);
        _extractor.Extract(input, res);
        return res;
    }
}