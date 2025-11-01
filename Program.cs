using spell.Core;
using spell.Modules;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace spell
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var examples = LoadExamples();
            var pipeline = BuildPipeline(examples);

            var root = new RootCommand("spell — CLI assistant");
            var rootText = new Argument<string>("text", () => string.Empty, "Natural-language command (shortcut for cast).");
            root.AddArgument(rootText);

            root.SetHandler(async (string text) =>
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    PrintUsage();
                }
                else
                {
                    ExecuteNlpCommand(text, pipeline);
                }

                await Task.CompletedTask;
            }, rootText);

            var castCmd = new Command("cast", "Run NLP pipeline using hybrid classifier")
            {
                new Argument<string>("text", "Natural-language command to process")
            };

            var castTextArg = (Argument<string>)castCmd.Arguments[0];

            castCmd.SetHandler(async (string text) =>
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("Error: cast requires a string argument. Example: spell cast \"remind me to call mom\"");
                }
                else
                {
                    ExecuteNlpCommand(text, pipeline);
                }

                await Task.CompletedTask;
            }, castTextArg);

            root.AddCommand(castCmd);

            Command CreateModuleCommand(string name)
            {
                var cmd = new Command(name, $"Operate with {name}s");
                var textArg = new Argument<string>("text", () => string.Empty, $"{name} text");
                var listOpt = new Option<bool>(new[] { "--list", "-l" }, $"Show {name} list");
                cmd.AddArgument(textArg);
                cmd.AddOption(listOpt);

                cmd.SetHandler(async (string text, bool list) =>
                {
                    if (list)
                    {
                        Console.WriteLine($"[{name}] List not implemented — implement storage to persist {name}s.");
                        await Task.CompletedTask;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        Console.WriteLine($"Usage: spell {name} \"<text>\"  OR  spell {name} --list");
                        await Task.CompletedTask;
                        return;
                    }

                    var intent = new IntentResult
                    {
                        Intent = name,
                        Confidence = 0.99,
                        RawText = text,
                        Entities = new Dictionary<string, object> { ["text"] = text }
                    };

                    try
                    {
                        switch (name)
                        {
                            case "reminder":
                                ReminderModule.Process(intent);
                                break;
                            case "note":
                                NotesModule.Process(intent);
                                break;
                            case "timer":
                                TimerModule.Process(intent);
                                break;
                            case "convert":
                                ConverterModule.Process(intent);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while executing {name}: {ex.Message}");
                    }

                    await Task.CompletedTask;
                }, textArg, listOpt);

                return cmd;
            }

            root.AddCommand(CreateModuleCommand("reminder"));
            root.AddCommand(CreateModuleCommand("note"));
            root.AddCommand(CreateModuleCommand("timer"));
            root.AddCommand(CreateModuleCommand("convert"));

            return await root.InvokeAsync(args);
        }

        static int ExecuteNlpCommand(string commandText, NlpPipeline pipeline)
        {
            Console.WriteLine($"Processing (NLP): \"{commandText}\"\n");

            IntentResult result;
            try
            {
                result = pipeline.Handle(commandText);
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
                        Console.WriteLine($"Unknown spell: \"{commandText}\"");
                        LogUnlabeled(commandText, result);
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

        static NlpPipeline BuildPipeline(Dictionary<string, string[]> examples)
        {
            var keywordClassifier = new KeywordIntentClassifier();
            var semanticClassifier = new SemanticIntentClassifier(examples)
            {
                AcceptanceThreshold = 0.8
            };
            var hybridClassifier = new HybridClassifier(keywordClassifier, semanticClassifier);
            return new NlpPipeline(hybridClassifier, new RecognizersEntityExtractor());
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  spell cast \"<natural-language-command>\"    Run NLP pipeline");
            Console.WriteLine("  spell reminder \"<text>\"                    Add a reminder (CLI mode)");
            Console.WriteLine("  spell reminder --list                       List reminders (CLI mode)");
            Console.WriteLine("  spell note \"<text>\"                        Add a note");
            Console.WriteLine("  spell note --list                            List notes");
            Console.WriteLine("  spell timer \"<text>\"                       Start a timer");
            Console.WriteLine("  spell timer --list                           List timers");
            Console.WriteLine("  spell convert \"<text>\"                     Run conversion");
            Console.WriteLine("  spell convert --list                         List conversions (placeholder)");
            Console.WriteLine("  spell \"<natural-language-command>\"          Shortcut for cast");
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
                }
                catch { }
            }

            return GetBuiltinExamples();
        }

        private static Dictionary<string, string[]> GetBuiltinExamples() => new()
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

        private static void LogUnlabeled(string input, IntentResult res)
        {
            try
            {
                var baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
                var logPath = Path.Combine(baseDir, "unlabeled.log");
                var line = $"{DateTime.UtcNow:O}\tIntent={res.Intent}\tConfidence={res.Confidence:F3}\tText={input}";
                File.AppendAllLines(logPath, new[] { line });
            }
            catch { }
        }
    }
}
