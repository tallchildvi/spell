using spell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Modules;


public interface IModule
{
    void Process(IntentResult command);
}