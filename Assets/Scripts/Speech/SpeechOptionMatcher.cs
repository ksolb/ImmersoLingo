using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

// Turns what Vosk heard into "which dialogue option did the player say?"
public static class SpeechOptionMatcher
{
    public const float MinScore = 0.6f;

    // "¡Hola! ¿Qué tal?" -> "hola que tal"
    public static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        string decomposed = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (char c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue; // drop accents
            sb.Append(char.IsLetterOrDigit(c) ? c : ' ');
        }
        return string.Join(" ", Words(sb.ToString()));
    }

    // Shared words / longer phrase's word count. 1.0 = exact match.
    public static float Score(string heard, string option)
    {
        var heardWords = new HashSet<string>(Words(Normalize(heard)));
        var optionWords = new HashSet<string>(Words(Normalize(option)));
        if (heardWords.Count == 0 || optionWords.Count == 0) return 0f;
        int hits = optionWords.Count(heardWords.Contains);
        return (float)hits / Math.Max(heardWords.Count, optionWords.Count);
    }

    // Index of the best-matching option, or -1 if nothing is close enough.
    public static int Match(IEnumerable<string> heardAlternatives, IList<string> options)
    {
        int best = -1;
        float bestScore = 0f;
        foreach (string heard in heardAlternatives)
        {
            for (int i = 0; i < options.Count; i++)
            {
                float s = Score(heard, options[i]);
                if (s > bestScore) { bestScore = s; best = i; }
            }
        }
        return bestScore >= MinScore ? best : -1;
    }

    [Serializable] private class VoskAlternative { public string text; }
    [Serializable] private class VoskResult { public string text; public VoskAlternative[] alternatives; }

    // Vosk returns JSON; pull out every non-empty transcription.
    public static List<string> ParseVoskResult(string json)
    {
        var texts = new List<string>();
        var result = JsonUtility.FromJson<VoskResult>(json);
        if (result == null) return texts;
        if (result.alternatives != null) texts.AddRange(result.alternatives.Select(a => a.text));
        if (!string.IsNullOrEmpty(result.text)) texts.Add(result.text);
        return texts.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
    }

    private static string[] Words(string s) => s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
}
