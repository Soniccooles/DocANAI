namespace DocANAI.Worker.Infrastructure.Chunking;

public sealed class ChunkSettings
{
    public const string SectionName = "Chunking";

    public string TokenizerModelPath { get; set; } = "Assets/tokenizers/multilingual-e5-base/sentencepiece.bpe.model";

    public int ChunkSizeTokens { get; set; } = 400;

    public int OverlapTokens { get; set; } = 40;

    public int MaxTailTokens { get; set; } = 30;
}