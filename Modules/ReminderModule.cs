using Microsoft.Recognizers.Text;
using spell.Core;
using System;


namespace spell.Modules;


public class ReminderModule : IModule
{
    public static void Process(IntentResult result)
    {
        if (result.Entities.TryGetValue("datetime", out var dtObj) && dtObj is List<ModelResult> dtList)
        {
            foreach (var dt in dtList)
            {
                Console.WriteLine($"Reminder datetime text: {dt.Text}");

                if (dt.Resolution != null && dt.Resolution.TryGetValue("values", out var valuesObj) && valuesObj is List<Dictionary<string, string>> values)
                {
                    foreach (var val in values)
                    {
                        if (val.TryGetValue("value", out var datetimeValue))
                        {
                            Console.WriteLine($"Parsed datetime: {datetimeValue}");
                        }
                    }
                }
            }
        }

        if (result.Entities.TryGetValue("text", out var text))
        {
            Console.WriteLine($"Reminder text: {text}");
        }
    }


    void IModule.Process(IntentResult command) => Process(command);
}