using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillMap.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class StringSimilarity
{
    public static string FindMostSimilar(string target, List<string> candidates)
    {
        if (string.IsNullOrWhiteSpace(target) || candidates == null || candidates.Count == 0)
            return null;

        string mostSimilar = null;
        double highestScore = double.MinValue;

        foreach (var candidate in candidates)
        {
            double score = CombinedSimilarityScore(target, candidate);
            if (score > highestScore)
            {
                highestScore = score;
                mostSimilar = candidate;
            }
        }

        return mostSimilar;
    }

    private static double CombinedSimilarityScore(string a, string b)
    {
        var tokensA = TokenizeAndSort(a);
        var tokensB = TokenizeAndSort(b);

        var normalizedA = string.Join(" ", tokensA);
        var normalizedB = string.Join(" ", tokensB);

        int levDist = LevenshteinDistance(normalizedA, normalizedB);
        int maxLen = Math.Max(normalizedA.Length, normalizedB.Length);

        double similarity = 1.0 - (double)levDist / maxLen;

        double jaccard = JaccardSimilarity(tokensA, tokensB);

        // Weighted average of both scores
        return (similarity * 0.5 + jaccard * 0.5);
    }

    private static List<string> TokenizeAndSort(string input)
    {
        var tokens = Regex.Matches(input.ToLower(), @"[a-z]+")
                          .Select(m => m.Value)
                          .OrderBy(t => t)
                          .ToList();
        return tokens;
    }

    private static double JaccardSimilarity(List<string> a, List<string> b)
    {
        var setA = new HashSet<string>(a);
        var setB = new HashSet<string>(b);

        int intersection = setA.Intersect(setB).Count();
        int union = setA.Union(setB).Count();

        return union == 0 ? 0 : (double)intersection / union;
    }

    private static int LevenshteinDistance(string a, string b)
    {
        int[,] matrix = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++) matrix[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) matrix[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1,
                             matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[a.Length, b.Length];
    }
}
