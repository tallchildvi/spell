using spell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spell.Core;

public class SemanticIntentClassifier : IIntentClassifier
{
    private readonly string[] _vocabulary;

    private readonly Dictionary<string, List<(double[] vec, double norm)>> _intentVectors;

    public double AcceptanceThreshold { get; set; } = 0.5;

    public SemanticIntentClassifier(Dictionary<string, string[]> examples)
    {
        if (examples == null) throw new ArgumentNullException(nameof(examples));

        _vocabulary = BuildVocabulary(examples);
        _intentVectors = new Dictionary<string, List<(double[] vec, double norm)>>();

        foreach (var kv in examples)
        {
            var list = new List<(double[] vec, double norm)>();
            foreach (var sample in kv.Value)
            {
                var v = Vectorize(sample);
                var norm = ComputeNorm(v);
                list.Add((v, norm));
            }
            _intentVectors[kv.Key] = list;
        }
    }

    public IntentResult Classify(string input)
    {
        var inputVec = Vectorize(input);
        var inputNorm = ComputeNorm(inputVec);

        double bestScore = 0.0;
        string bestIntent = "unknown";

        foreach (var kv in _intentVectors)
        {
            foreach (var ex in kv.Value)
            {
                var score = CosineSimilarityPrecomputed(inputVec, inputNorm, ex.vec, ex.norm);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestIntent = kv.Key;
                }
            }
        }

        var finalIntent = bestScore >= AcceptanceThreshold ? bestIntent : "unknown";

        return new IntentResult
        {
            Intent = finalIntent,
            Confidence = bestScore,
            RawText = input
        };
    }


    private string[] BuildVocabulary(Dictionary<string, string[]> examples)
    {
        var set = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var kv in examples)
        {
            foreach (var s in kv.Value)
            {
                var toks = Tokenize(s);
                foreach (var t in toks) set.Add(t);
            }
        }
        return set.ToArray();
    }

    private double[] Vectorize(string text)
    {
        var tokens = Tokenize(text);
        var set = new HashSet<string>(tokens, StringComparer.InvariantCultureIgnoreCase);
        var vec = new double[_vocabulary.Length];
        for (int i = 0; i < _vocabulary.Length; i++)
            vec[i] = set.Contains(_vocabulary[i]) ? 1.0 : 0.0; // binary presence
        return vec;
    }

    private static string[] Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Array.Empty<string>();
        var split = text
            .ToLowerInvariant()
            .Split(new[] { ' ', '\t', '\n', '.', ',', '!', '?', ';', ':', '(', ')', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries);
        return split.Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
    }

    private static double ComputeNorm(double[] v)
    {
        double s = 0.0;
        for (int i = 0; i < v.Length; i++) s += v[i] * v[i];
        return Math.Sqrt(s) + 1e-12; // avoid zero
    }

    private static double CosineSimilarityPrecomputed(double[] a, double normA, double[] b, double normB)
    {
        if (a.Length != b.Length) return 0.0;
        double dot = 0.0;
        for (int i = 0; i < a.Length; i++) dot += a[i] * b[i];
        return dot / (normA * normB);
    }
}