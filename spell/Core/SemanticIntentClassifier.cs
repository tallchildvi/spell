using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace spell.Core
{
    public class SemanticIntentClassifier : IIntentClassifier
    {
        private readonly Dictionary<string, string[]> _examples;
        private readonly Dictionary<string, List<double[]>> _intentVectors = new();
        private readonly List<string> _vocabulary = new();
        private readonly Dictionary<string, double> _idf = new();

        public double AcceptanceThreshold { get; set; } = 0.55; 

        public SemanticIntentClassifier(Dictionary<string, string[]> examples)
        {
            _examples = examples ?? throw new ArgumentNullException(nameof(examples));
            BuildVocabulary();
            ComputeIDF();
            BuildExampleVectors();
        }

        public IntentResult Classify(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new IntentResult { Intent = "unknown", Confidence = 0.0, RawText = input };

            var inputVector = ToVector(input);

            string bestIntent = "unknown";
            double bestSim = 0.0;

            foreach (var kv in _intentVectors)
            {
                foreach (var vec in kv.Value)
                {
                    double sim = CosineSimilarity(inputVector, vec);
                    if (sim > bestSim)
                    {
                        bestSim = sim;
                        bestIntent = kv.Key;
                    }
                }
            }

            if (bestSim < AcceptanceThreshold)
            {
                return new IntentResult { Intent = "unknown", Confidence = bestSim, RawText = input };
            }

            return new IntentResult { Intent = bestIntent, Confidence = bestSim, RawText = input };
        }
        private void BuildVocabulary()
        {
            var vocabSet = new HashSet<string>();
            foreach (var group in _examples.Values)
            {
                foreach (var text in group)
                {
                    foreach (var word in Tokenize(text))
                        vocabSet.Add(word);
                }
            }

            _vocabulary.Clear();
            _vocabulary.AddRange(vocabSet.OrderBy(w => w));
        }

        private void ComputeIDF()
        {
            int docCount = _examples.Values.Sum(list => list.Length);
            var docFreq = new Dictionary<string, int>();

            foreach (var word in _vocabulary)
                docFreq[word] = 0;

            foreach (var group in _examples.Values)
            {
                foreach (var text in group)
                {
                    var uniqueWords = Tokenize(text).Distinct();
                    foreach (var w in uniqueWords)
                        if (docFreq.ContainsKey(w))
                            docFreq[w]++;
                }
            }

            foreach (var w in _vocabulary)
            {
                double df = docFreq[w];
                _idf[w] = Math.Log((1 + docCount) / (1 + df)) + 1;
            }
        }

        private void BuildExampleVectors()
        {
            _intentVectors.Clear();
            foreach (var kv in _examples)
            {
                var list = new List<double[]>();
                foreach (var text in kv.Value)
                {
                    var vec = ToVector(text);
                    list.Add(vec);
                }
                _intentVectors[kv.Key] = list;
            }
        }

        private double[] ToVector(string text)
        {
            var tokens = Tokenize(text);
            var tf = new Dictionary<string, double>();
            foreach (var w in tokens)
                tf[w] = tf.GetValueOrDefault(w, 0) + 1;

            int n = _vocabulary.Count;
            double[] vec = new double[n];
            for (int i = 0; i < n; i++)
            {
                var word = _vocabulary[i];
                if (tf.TryGetValue(word, out double f))
                {
                    double idf = _idf.GetValueOrDefault(word, 1.0);
                    vec[i] = (f / tokens.Count) * idf;
                }
                else
                {
                    vec[i] = 0;
                }
            }
            return vec;
        }

        private static double CosineSimilarity(double[] a, double[] b)
        {
            double dot = 0, magA = 0, magB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }

            double denom = Math.Sqrt(magA) * Math.Sqrt(magB);
            return denom == 0 ? 0 : dot / denom;
        }

        private static readonly HashSet<string> StopWords = new()
        {
            "the", "a", "to", "for", "at", "be", "on", "of", "in", "me", "my", "is", "am", "are"
        };
        private static List<string> Tokenize(string text)
        {
            text = text.ToLowerInvariant();
            text = Regex.Replace(text, @"[^a-z0-9\s]", " ");
            var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Where(w => w.Length > 1 && !StopWords.Contains(w)).ToList();
        }
    }
}