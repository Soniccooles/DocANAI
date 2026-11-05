using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

public sealed class DocumentChunkEntityTypeConfiguration : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable(nameof(DocumentChunk).ToSnakeCase());
        
        builder.HasId();
        builder.HasAudits();
        builder.HasIndex(x => new { x.SourceDocumentId, x.ChunkIndex }).IsUnique();
        
        builder.Property(x => x.SourceDocumentId)
            .IsRequired()
            .HasColumnName(nameof(DocumentChunk.SourceDocumentId).ToSnakeCase());
        
        builder.Property(x => x.Content)
            .IsRequired()
            .HasColumnType("text")
            .HasColumnName(nameof(DocumentChunk.Content).ToSnakeCase());
        
        builder.Property(x => x.ChunkIndex)
            .IsRequired()
            .HasColumnName(nameof(DocumentChunk.ChunkIndex).ToSnakeCase());
        
        builder.Property(x => x.PageNumber)
            .IsRequired(false)
            .HasColumnName(nameof(DocumentChunk.PageNumber).ToSnakeCase());
        
        builder.Property(x => x.Embedding)
            .IsRequired(false)
            .HasColumnType("vector(768)")
            .HasColumnName(nameof(DocumentChunk.Embedding).ToSnakeCase());

        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops")
            .HasStorageParameter("m", 16)
            .HasStorageParameter("ef_construction", 64);
        
        builder.HasOne<SourceDocument>()
            .WithMany()
            .HasForeignKey(x => x.SourceDocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}