using System;
using spell.Core;


namespace spell.Modules;


public class NotesModule : IModule
{
    public static void Process(IntentResult command)
    {
        Console.WriteLine($"[Note] Intent confidence: {command.Confidence}");
        var text = command.Entities.GetValueOrDefault("text")?.ToString() ?? command.RawText;
        Console.WriteLine($"Note saved: {text}");
    }


    void IModule.Process(IntentResult command) => Process(command);
}