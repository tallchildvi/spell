using System;
using System.Collections.Generic;
using spell.Core;
using Microsoft.Recognizers.Text;


namespace spell.Modules;


public class TimerModule : IModule
{
    public static void Process(IntentResult command)
    {
        Console.WriteLine($"[Timer] Intent confidence: {command.Confidence}");
        if (command.Entities.TryGetValue("datetime", out var dtObj) && dtObj is IList<Microsoft.Recognizers.Text.ModelResult> list && list.Count > 0)
        {
            foreach (var m in list)
            {
                Console.WriteLine($"Found datetime/entity: type={m.TypeName} text='{m.Text}'");
            }
        }
        else
        {
            Console.WriteLine($"Timer started (raw): {command.Entities.GetValueOrDefault("text")}");
        }
    }


    void IModule.Process(IntentResult command) => Process(command);
}