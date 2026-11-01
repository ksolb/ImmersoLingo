using System.Collections.Generic;
using NUnit.Framework;

public class SpeechOptionMatcherTests
{
    [Test]
    public void Normalize_StripsAccentsPunctuationAndCase()
    {
        Assert.AreEqual("hola que tal", SpeechOptionMatcher.Normalize("¡Hola! ¿Qué tal?"));
    }

    [Test]
    public void Match_ExactPhrase_ReturnsThatOption()
    {
        var options = new List<string> { "Un café, por favor", "Nada, gracias" };
        Assert.AreEqual(1, SpeechOptionMatcher.Match(new[] { "nada gracias" }, options));
    }

    [Test]
    public void Match_ExtraFillerWord_StillMatches()
    {
        var options = new List<string> { "Un café, por favor", "Nada, gracias" };
        Assert.AreEqual(0, SpeechOptionMatcher.Match(new[] { "eh un cafe por favor" }, options));
    }

    [Test]
    public void Match_PicksLongerOption_WhenFullySpoken()
    {
        var options = new List<string> { "Sí", "Sí, por favor" };
        Assert.AreEqual(1, SpeechOptionMatcher.Match(new[] { "si por favor" }, options));
    }

    [Test]
    public void Match_UnrelatedSpeech_ReturnsMinusOne()
    {
        var options = new List<string> { "Un café, por favor", "Nada, gracias" };
        Assert.AreEqual(-1, SpeechOptionMatcher.Match(new[] { "donde esta el bano" }, options));
    }

    [Test]
    public void Match_ChecksEveryAlternative()
    {
        var options = new List<string> { "Un café, por favor", "Nada, gracias" };
        Assert.AreEqual(1, SpeechOptionMatcher.Match(new[] { "nado gracia", "nada gracias" }, options));
    }

    [Test]
    public void ParseVoskResult_ReadsAlternatives()
    {
        string json = "{\"alternatives\":[{\"confidence\":250.1,\"text\":\"hola\"},{\"confidence\":200.0,\"text\":\"ola\"}]}";
        CollectionAssert.AreEqual(new[] { "hola", "ola" }, SpeechOptionMatcher.ParseVoskResult(json));
    }

    [Test]
    public void ParseVoskResult_EmptyText_ReturnsEmptyList()
    {
        Assert.AreEqual(0, SpeechOptionMatcher.ParseVoskResult("{\"text\":\"\"}").Count);
    }
}