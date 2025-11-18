using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Core;


public interface IIntentClassifier
{
    IntentResult Classify(string input);
}