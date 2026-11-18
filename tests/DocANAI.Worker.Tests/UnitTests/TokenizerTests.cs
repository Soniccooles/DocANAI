using Microsoft.ML.Tokenizers;

namespace DocANAI.Worker.Tests.UnitTests;

public class TokenizerTests
{
    private const string ModelPath = "Infrastructure/Chunking/sentencepiece.bpe.model";

    // Сравнение идет с AutoTokenizer.from_pretrained("intfloat/multilingual-e5-base")
    [TestCase("english.txt", 559)]
    [TestCase("table.txt", 443)]
    public void CountTokens_MatchesReferenceTokenizer(string fileName, int expectedTokens)
    {
        using var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, ModelPath));
        var tokenizer = SentencePieceTokenizer.Create(stream, addBeginningOfSentence: true, addEndOfSentence: true);
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "TestData", fileName));

        var bare = tokenizer.CountTokens(text, addBeginningOfSentence: false, addEndOfSentence: false);
        var full = tokenizer.CountTokens(text);
        var prefix = tokenizer.CountTokens("passage: " + text, addBeginningOfSentence: false, addEndOfSentence: false) - bare;

        Assert.Multiple(() =>
        {
            Assert.That(bare, Is.EqualTo(expectedTokens));
            Assert.That(full, Is.EqualTo(expectedTokens + 2));
            Assert.That(prefix, Is.EqualTo(2));
        });
    }
}