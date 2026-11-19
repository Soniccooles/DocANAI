using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Microsoft.ML.Tokenizers;

namespace DocANAI.Worker.Infrastructure.Chunking;

public sealed partial class DocumentChunker : IDocumentChunker
{
    public const int ModelMaxTokens = 512;
    public const string PassagePrefix = "passage: ";
    
    private const bool AddBeginningOfSentence = true;
    private const bool AddEndOfSentence = true;
    
    private const char SpaceSymbol = '\u2581';

    private static readonly (Regex Splitter, string Separator)[] Levels =
    [
        (ParagraphRegex(), "\n"),
        (SentenceRegex(), " "),
        (WordRegex(), " ")
    ];

    private readonly SentencePieceTokenizer _tokenizer;
    private readonly ChunkSettings _settings;

    public DocumentChunker(IOptions<ChunkSettings> options)
    {
        _settings = options.Value;

        var modelPath = Path.Combine(AppContext.BaseDirectory, _settings.TokenizerModelPath);
        using var stream = File.OpenRead(modelPath);
        _tokenizer = SentencePieceTokenizer.Create(stream, AddBeginningOfSentence, AddEndOfSentence);
    }

    public IReadOnlyList<TextChunk> Split(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return [];

        var accumulator = new Accumulator(_settings.ChunkSizeTokens);
        SplitByLevel(text, 0, accumulator);
        accumulator.Flush();

        var parts = accumulator.Parts;
        AttachSmallTail(parts);

        var chunks = new List<TextChunk>(parts.Count);
        for (var i = 0; i < parts.Count; i++)
        {
            var overlap = i == 0 ? string.Empty : GetOverlap(parts[i - 1]);
            var content = overlap.Length == 0 ? parts[i] : $"{overlap} {parts[i]}";
            chunks.Add(new TextChunk(i, content));
        }

        return chunks;
    }

    private void SplitByLevel(string text, int level, Accumulator accumulator)
    {
        var (splitter, separator) = Levels[level];

        foreach (var rawPiece in splitter.Split(text))
        {
            var piece = rawPiece.Trim();
            if (piece.Length == 0) continue;

            var tokens = CountTokens(piece);

            if (tokens <= _settings.ChunkSizeTokens)
                accumulator.Add(piece, tokens, separator);
            else if (level + 1 < Levels.Length)
                SplitByLevel(piece, level + 1, accumulator);
            else
                SplitByTokenLimit(piece, accumulator);
        }
    }

    private void SplitByTokenLimit(string word, Accumulator accumulator)
    {
        var rest = word;
        var normalize = true;

        while (rest.Length > 0)
        {
            var index = _tokenizer.GetIndexByTokenCount(
                rest,
                addBeginningOfSentence: false,
                addEndOfSentence: false,
                _settings.ChunkSizeTokens,
                out var normalized,
                out var tokens,
                considerNormalization: normalize);

            rest = normalized ?? rest;
            if (index == 0) throw new InvalidOperationException("Token limit is too small to split the text");

            accumulator.Add(Denormalize(rest[..index]), tokens, " ");
            var remainder = rest[index..];
            rest = remainder.Length == 0 ? string.Empty : SpaceSymbol + remainder;
            normalize = false;
        }
    }

    private string GetOverlap(string previous)
    {
        var index = _tokenizer.GetIndexByTokenCountFromEnd(
            previous,
            addBeginningOfSentence: false,
            addEndOfSentence: false,
            _settings.OverlapTokens,
            considerNormalization: true,
            out var normalized,
            out _);

        normalized ??= previous;
        
        if (index > 0 && index < normalized.Length
            && normalized[index] != SpaceSymbol && normalized[index - 1] != SpaceSymbol)
        {
            var nextWord = normalized.IndexOf(SpaceSymbol, index);
            if (nextWord >= 0) index = nextWord;
        }

        return Denormalize(normalized[index..]);
    }

    private void AttachSmallTail(List<string> parts)
    {
        if (parts.Count < 2 || CountTokens(parts[^1]) > _settings.MaxTailTokens) return;

        parts[^2] = $"{parts[^2]} {parts[^1]}";
        parts.RemoveAt(parts.Count - 1);
    }

    private int CountTokens(string text) =>
        _tokenizer.CountTokens(text, addBeginningOfSentence: false, addEndOfSentence: false);

    private static string Denormalize(string text) => text.Replace(SpaceSymbol, ' ').Trim();

    [GeneratedRegex(@"\n+")]
    private static partial Regex ParagraphRegex();

    [GeneratedRegex(@"(?<=[.!?…])\s+")]
    private static partial Regex SentenceRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WordRegex();

    private sealed class Accumulator(int limit)
    {
        private readonly StringBuilder _text = new();
        private int _tokens;

        public List<string> Parts { get; } = [];

        public void Add(string piece, int tokens, string separator)
        {
            if (_tokens + tokens > limit) Flush();

            if (_text.Length > 0) _text.Append(separator);
            _text.Append(piece);
            _tokens += tokens;
        }

        public void Flush()
        {
            if (_text.Length == 0) return;

            Parts.Add(_text.ToString());
            _text.Clear();
            _tokens = 0;
        }
    }
}