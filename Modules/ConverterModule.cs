using System;
using System.Collections.Generic;
using spell.Core;
using Microsoft.Recognizers.Text.NumberWithUnit;


namespace spell.Modules;


public  class ConverterModule : IModule
{
    public static void Process(IntentResult command)
    {
        Console.WriteLine($"[Converter] Intent confidence: {command.Confidence}");


        if (command.Entities.TryGetValue("units", out var unitsObj) && unitsObj is IList<Microsoft.Recognizers.Text.ModelResult> units && units.Count > 0)
        {
            foreach (var u in units)
            {
                Console.WriteLine($"Unit result: type={u.TypeName} text='{u.Text}' resolution={u.Resolution}");
            }
        }
        else
        {
            Console.WriteLine($"Conversion request (raw): {command.Entities.GetValueOrDefault("text")}");
        }
    }


    void IModule.Process(IntentResult command) => Process(command);
}
