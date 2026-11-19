using System.Text.RegularExpressions;
using DocANAI.Worker.Infrastructure.Chunking;
using Microsoft.Extensions.Options;
using Microsoft.ML.Tokenizers;

namespace DocANAI.Worker.Tests.UnitTests;

public class DocumentChunkerTests
{
    
    private static readonly SentencePieceTokenizer Tokenizer = CreateTokenizer();

    private static SentencePieceTokenizer CreateTokenizer()
    {
        var modelPath = Path.Combine(AppContext.BaseDirectory, new ChunkSettings().TokenizerModelPath);
        using var stream = File.OpenRead(modelPath);
        return SentencePieceTokenizer.Create(stream, addBeginningOfSentence: true, addEndOfSentence: true);
    }
    
    private static DocumentChunker CreateChunker(int chunkSize, int overlap, int maxTail) =>
        new(Options.Create(new ChunkSettings
        {
            ChunkSizeTokens = chunkSize,
            OverlapTokens = overlap,
            MaxTailTokens = maxTail
        }));
    
    private static int CountTokens(string text) =>
        Tokenizer.CountTokens(text, addBeginningOfSentence: false, addEndOfSentence: false);
    
    private static string ReadTestData(string fileName) =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "TestData", fileName));

    [Test]
    public void split_whitespace_text_returns_no_chunks()
    {
        var chunker = CreateChunker(30, 0, 0);
        var text = "    \n    ";

        var chunks = chunker.Split(text);
        
        Assert.That(chunks, Is.Empty);
    }

    [TestCase("russian_article.txt")]
    [TestCase("english.txt")]
    [TestCase("no_spaces.txt")]
    public void split_long_text_each_chunk_fits_limit(string fileName)
    {
        var chunker = CreateChunker(30, 0, 0);
        var text = ReadTestData(fileName);
        
        var chunks = chunker.Split(text);
        
        Assert.That(chunks, Has.Count.GreaterThan(1));
        foreach (var chunk in chunks)
        {
            Assert.That(CountTokens(chunk.Text), Is.LessThanOrEqualTo(30));
        }
    }

    [TestCase("russian_article.txt")]
    [TestCase("english.txt")]
    [TestCase("no_spaces.txt")]
    public void split_long_text_covers_whole_text(string fileName)
    {
        var chunker = CreateChunker(30, 0, 0);
        var text = ReadTestData(fileName);
        
        var chunks = chunker.Split(text);
        var newText = string.Concat(chunks.Select(chunk => chunk.Text));
        var expected = Regex.Replace(text, @"\s+", string.Empty);
        var actual = Regex.Replace(newText, @"\s+", string.Empty);
        
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCase("english.txt")]
    [TestCase("no_spaces.txt")]
    [TestCase("russian_article.txt")]
    public void split_default_settings_chunk_fits_model_window(string fileName)
    {
        var chunker = CreateChunker(400, 40, 30);
        var text = ReadTestData(fileName);
        
        var chunks = chunker.Split(text);
        
        Assert.That(chunks, Has.Count.GreaterThan(1));
        foreach (var chunk in chunks)
        {
            var tokens = Tokenizer.CountTokens(DocumentChunker.PassagePrefix + chunk.Text);
            Assert.That(tokens, Is.LessThanOrEqualTo(DocumentChunker.ModelMaxTokens));
        }
    }

    [Test]
    public void split_with_overlap_chunk_starts_with_end_of_previous()
    {
        var chunkerWithoutOverlap = CreateChunker(30, 0, 0);
        var chunkerWithOverlap = CreateChunker(30, 5, 0);
        var text = ReadTestData("english.txt");
        
        var parts = chunkerWithoutOverlap.Split(text);
        var chunks = chunkerWithOverlap.Split(text);

        Assert.That(parts, Has.Count.EqualTo(chunks.Count));

        for (int i = 1; i < parts.Count; i++)
        {
            Assert.That(chunks[i].Text, Does.EndWith(parts[i].Text));
            var overlap = chunks[i].Text[..^parts[i].Text.Length].Trim();
            Assert.That(overlap, Is.Not.Empty);
            Assert.That(CountTokens(overlap), Is.LessThanOrEqualTo(5));
        }
    }

    [Test]
    public void split_small_tail_is_attached_to_previous_chunk()
    {
        var text = ReadTestData("english.txt");

        var parts = CreateChunker(30, 0, 0).Split(text);
        var tailTokens = CountTokens(parts[^1].Text);
        var chunks = CreateChunker(30, 0, tailTokens).Split(text);
        
        Assert.That(chunks, Has.Count.EqualTo(parts.Count - 1));
        Assert.That(chunks[^1].Text, Is.EqualTo($"{parts[^2].Text} {parts[^1].Text}"));
    }
}