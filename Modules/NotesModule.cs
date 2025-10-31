using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Modules;

public static class NotesModule
{
    public static void Process(string input)
    {
        Console.WriteLine($"Note saved: {input}");
        // TODO: save note text to file
    }
}
