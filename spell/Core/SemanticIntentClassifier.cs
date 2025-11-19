/// <file>
/// <summary>
/// SemanticIntentClassifier.cs - Semantic similarity-based intent classifier
/// </summary>
/// </file>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace spell.Core
/// <summary>
/// Semantic classifier using TF-IDF and cosine similarity
/// </summary>
/// <remarks>
/// This classifier builds a vector space model from training examples and uses
/// cosine similarity to find the closest matching intent. More flexible than
/// keyword matching but requires training data.
/// </remarks>
{
    public class SemanticIntentClassifier : IIntentClassifier
    {
        /// <summary>
        /// Training examples organized by intent
        /// </summary>
        private readonly Dictionary<string, string[]> _examples;
        /// <summary>
        /// Pre-computed TF-IDF vectors for each intent's examples
        /// </summary>
        private readonly Dictionary<string, List<double[]>> _intentVectors = new();
        /// <summary>
        /// Vocabulary of unique words across all examples
        /// </summary>
        private readonly List<string> _vocabulary = new();
        /// <summary>
        /// Inverse document frequency scores for each word
        /// </summary>
        private readonly Dictionary<string, double> _idf = new();

        /// <summary>
        /// Gets or sets the minimum similarity threshold for classification
        /// </summary>
        /// <value>Threshold value between 0.0 and 1.0, defaults to 0.55</value>
        /// <remarks>
        /// Results below this threshold are classified as "unknown"
        /// </remarks>
        public double AcceptanceThreshold { get; set; } = 0.55;

        /// <summary>
        /// Initializes a new instance of SemanticIntentClassifier with training examples
        /// </summary>
        /// <param name="examples">Dictionary of intent names to arrays of example phrases</param>
        /// <exception cref="ArgumentNullException">Thrown when examples is null</exception>
        /// <example>
        /// <code>
        /// var examples = new Dictionary&lt;string, string[]&gt;
        /// {
        ///     ["reminder"] = new[] { "remind me to...", "set a reminder..." },
        ///     ["timer"] = new[] { "set timer for...", "start countdown..." }
        /// };
        /// var classifier = new SemanticIntentClassifier(examples)
        /// {
        ///     AcceptanceThreshold = 0.6
        /// };
        /// </code>
        /// </example>
        /// 
        public SemanticIntentClassifier(Dictionary<string, string[]> examples)
        {
            _examples = examples ?? throw new ArgumentNullException(nameof(examples));
            BuildVocabulary();
            ComputeIDF();
            BuildExampleVectors();
        }

        /// <summary>
        /// Classifies input text using semantic similarity
        /// </summary>
        /// <param name="input">Natural language text to classify</param>
        /// <returns>IntentResult with matched intent and similarity score as confidence</returns>
        /// <exception cref="ArgumentException">Not thrown, handles empty input gracefully</exception>
        /// <example>
        /// <code>
        /// var result = classifier.Classify("please remind me about the meeting");
        /// // result.Intent may be "reminder" if similarity > threshold
        /// // result.Confidence is the cosine similarity score
        /// </code>
        /// </example>
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

        /// <summary>
        /// Builds vocabulary from all training examples
        /// </summary>
        /// <remarks>
        /// Tokenizes all examples and creates a sorted list of unique words
        /// </remarks>
        /// 
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

        /// <summary>
        /// Computes IDF (Inverse Document Frequency) scores for vocabulary
        /// </summary>
        /// <remarks>
        /// Uses formula: log((1 + docCount) / (1 + documentFrequency)) + 1
        /// Higher scores for rarer words across documents
        /// </remarks>
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

        /// <summary>
        /// Builds TF-IDF vectors for all training examples
        /// </summary>
        /// <remarks>
        /// Pre-computes vectors for fast similarity comparison during classification
        /// </remarks>

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

        /// <summary>
        /// Converts text to TF-IDF vector representation
        /// </summary>
        /// <param name="text">Text to vectorize</param>
        /// <returns>TF-IDF vector with dimension equal to vocabulary size</returns>
        /// <remarks>
        /// TF (Term Frequency) = word_count / total_words
        /// TF-IDF = TF * IDF
        /// </remarks>
        /// 
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

        /// <summary>
        /// Computes cosine similarity between two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Similarity score between 0.0 and 1.0</returns>
        /// <remarks>
        /// Formula: dot(a, b) / (magnitude(a) * magnitude(b))
        /// Returns 0 if either vector has zero magnitude
        /// </remarks>
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

        /// <summary>
        /// Common English stop words to filter out during tokenization
        /// </summary>
        ///  
        private static readonly HashSet<string> StopWords = new()
        {
            "the", "a", "to", "for", "at", "be", "on", "of", "in", "me", "my", "is", "am", "are"
        };

        /// <summary>
        /// Tokenizes text into normalized words
        /// </summary>
        /// <param name="text">Text to tokenize</param>
        /// <returns>List of lowercase words with stop words and short words removed</returns>
        /// <remarks>
        /// Process: lowercase → remove punctuation → split → filter stop words and single chars
        /// </remarks>
        private static List<string> Tokenize(string text)
        {
            text = text.ToLowerInvariant();
            text = Regex.Replace(text, @"[^a-z0-9\s]", " ");
            var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Where(w => w.Length > 1 && !StopWords.Contains(w)).ToList();
        }
    }
}