using System;
using spell.Core;

namespace spell;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a command, e.g. spell \"remind me to call mom tomorrow\"");
            return 1;
        }

        var input = string.Join(" ", args);
        SpellRouter.Handle(input);
        return 0;
    }
}
