namespace DocANAI.Worker.Infrastructure.Chunking;

public interface IDocumentChunker
{
    IReadOnlyList<TextChunk> Split(string text);
}