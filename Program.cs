using System;
using spell.Core;
using spell.Modules;


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


        var pipeline = new NlpPipeline(new KeywordIntentClassifier(), new RecognizersEntityExtractor());
        var result = pipeline.Handle(input);


        Console.WriteLine($"Detected intent: {result.Intent} (confidence {result.Confidence})\n");


        switch (result.Intent)
        {
            case "reminder":
                ReminderModule.Process(result);
                break;
            case "note":
                NotesModule.Process(result);
                break;
            case "timer":
                TimerModule.Process(result);
                break;
            case "convert":
                ConverterModule.Process(result);
                break;
            default:
                Console.WriteLine($"Unknown spell: \"{input}\"");
                break;
        }


        return 0;
    }
}