using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Modules;

public static class ConverterModule
{
    public static void Process(string input)
    {
        Console.WriteLine($"Conversion request: {input}");
        // TODO: detect units and perform conversion
    }
}
