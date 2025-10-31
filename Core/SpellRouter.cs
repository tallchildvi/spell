using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using spell.Modules;
namespace spell.Core;

public static class SpellRouter
{
    public static void Handle(string input)
    {
        var lower = input.ToLower();

        if (lower.Contains("remind"))
        {
            ReminderModule.Process(input);
        }
        else if (lower.Contains("note") || lower.Contains("remember"))
        {
            NotesModule.Process(input);
        }
        else if (lower.Contains("convert"))
        {
            ConverterModule.Process(input);
        }
        else if (lower.Contains("timer"))
        {
            TimerModule.Process(input);
        }
        else
        {
            Console.WriteLine($"Unknown spell: \"{input}\"");
        }
    }
}
