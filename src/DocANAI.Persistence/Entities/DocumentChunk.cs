using DocANAI.Persistence.Abstractions;
using DocANAI.Persistence.ValueObjects;
using JetBrains.Annotations;
using Pgvector;

namespace DocANAI.Persistence.Entities;

public sealed class DocumentChunk : AuditableEntity<IdOf<DocumentChunk>>
{
    public IdOf<SourceDocument> SourceDocumentId { get; set; }
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = null!;
    public int? PageNumber { get; set; }
    public Vector? Embedding { get; set; } = null!; // 768d
    
    private DocumentChunk(
        IdOf<DocumentChunk> id,
        IdOf<SourceDocument> sourceDocumentId,
        int chunkIndex,
        string content,
        int? pageNumber,
        Vector? embedding)
    {
        Id = id; 
        SourceDocumentId = sourceDocumentId;
        ChunkIndex = chunkIndex;
        Content = content;
        PageNumber = pageNumber;
        Embedding = embedding;
    }

    public static DocumentChunk Create(
        IdOf<DocumentChunk> id,
        IdOf<SourceDocument> sourceDocumentId,
        int chunkIndex,
        string content,
        int? pageNumber,
        Vector? embedding)
        => new(id, sourceDocumentId, chunkIndex, content, pageNumber, embedding);

    [UsedImplicitly]
#pragma warning disable CS8618
    private DocumentChunk() { }
#pragma warning restore CS8618
}