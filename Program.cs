using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using spell.Core;
using spell.Modules;

namespace spell
{
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

            var examples = LoadExamples();

            var keywordClassifier = new KeywordIntentClassifier();
            var semanticClassifier = new SemanticIntentClassifier(examples);

            semanticClassifier.AcceptanceThreshold = 0.55; 

            var hybridClassifier = new HybridClassifier(keywordClassifier, semanticClassifier);

            var pipeline = new NlpPipeline(hybridClassifier, new RecognizersEntityExtractor());

            IntentResult result;
            try
            {
                result = pipeline.Handle(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during pipeline handling: {ex.Message}");
                return 2;
            }

            Console.WriteLine($"Detected intent: {result.Intent} (confidence {result.Confidence})\n");

            try
            {
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
                        LogUnlabeled(input, result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while executing module: {ex.Message}");
                return 3;
            }

            return 0;
        }

        private static Dictionary<string, string[]> LoadExamples()
        {
            var baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
            var examplesDir = Path.Combine(baseDir, "Core");
            var examplesPath = Path.Combine(examplesDir, "examples.json");

            if (File.Exists(examplesPath))
            {
                try
                {
                    var txt = File.ReadAllText(examplesPath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(txt);
                    if (dict != null && dict.Count > 0)
                    {
                        Console.WriteLine($"Loaded examples from {examplesPath} (intents: {dict.Count})");
                        return dict;
                    }
                    else
                    {
                        Console.WriteLine($"examples.json exists but is empty or invalid. Using builtin examples.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read/parse examples.json: {ex.Message}. Using builtin examples.");
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(examplesDir);
                    var defaultExamples = GetBuiltinExamples();
                    var json = JsonSerializer.Serialize(defaultExamples, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(examplesPath, json);
                    Console.WriteLine($"No examples.json found. A default one was written to: {examplesPath}");
                }
                catch{}
            }

            return GetBuiltinExamples();
        }

        private static Dictionary<string, string[]> GetBuiltinExamples()
        {
            return new Dictionary<string, string[]>
            {
                ["reminder"] = new[]
                {
                    "remind me to call mom tomorrow",
                    "set a reminder for 8am",
                    "reminder to buy milk",
                    "remind me about the meeting at 9",
                    "please remind me to pay bills"
                },
                ["note"] = new[]
                {
                    "take note of this",
                    "remember that I have a meeting",
                    "save this note",
                    "note: buy coffee",
                    "write this down"
                },
                ["timer"] = new[]
                {
                    "set timer for 10 minutes",
                    "start countdown for 30 seconds",
                    "run timer for 5 minutes",
                    "timer 1 minute",
                    "countdown 2 hours"
                },
                ["convert"] = new[]
                {
                    "convert 5 kilograms to grams",
                    "how many meters are in 2 kilometers",
                    "change 10 inches to centimeters",
                    "convert 100 usd to eur",
                    "what is 37 celsius in fahrenheit"
                }
            };
        }
        private static void LogUnlabeled(string input, IntentResult res)
        {
            try
            {
                var baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
                var logPath = Path.Combine(baseDir, "unlabeled.log");
                var line = $"{DateTime.UtcNow:O}\tIntent={res.Intent}\tConfidence={res.Confidence:F3}\tText={input}";
                File.AppendAllLines(logPath, new[] { line });
            }
            catch{}
        }
    }
}
