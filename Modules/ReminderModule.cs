using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Modules;

public static class ReminderModule
{
    public static void Process(string input)
    {
        Console.WriteLine($"Reminder created: {input}");
        // TODO: parse time, save to JSON
    }
}
