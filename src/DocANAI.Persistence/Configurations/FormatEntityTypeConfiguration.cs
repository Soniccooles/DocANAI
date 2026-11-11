using DocANAI.Persistence.Entities;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

public class FormatEntityTypeConfiguration : IEntityTypeConfiguration<Format>
{
    private const int MaxFileSize = 100 * 1024 * 1024;
    
    public void Configure(EntityTypeBuilder<Format> builder)
    {
        builder.ToTable(nameof(Format).ToSnakeCase());
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(20)
            .HasColumnName(nameof(Format.Extension).ToSnakeCase());

        builder.Property(x => x.MaxSize)
            .IsRequired()
            .HasColumnName(nameof(Format.MaxSize).ToSnakeCase());

        builder.Property(x => x.MimeType)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName(nameof(Format.MimeType).ToSnakeCase());
        
        builder.HasData(Format.Create(".pdf", MaxFileSize, "application/pdf"),
            Format.Create(".docx", MaxFileSize, "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
            Format.Create(".xlsx", MaxFileSize, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
    }
}