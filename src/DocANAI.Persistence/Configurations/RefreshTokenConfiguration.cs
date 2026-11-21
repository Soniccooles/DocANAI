using DocANAI.Persistence.Entities.Users;
using DocANAI.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocANAI.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable(nameof(RefreshToken).ToSnakeCase());

        builder.HasId();
        builder.HasAudits();
        
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnName(nameof(RefreshToken.Token).ToSnakeCase());
        
        builder.HasIndex(rt => rt.Token)
            .IsUnique();
        
        builder.Property(rt => rt.ExpiresAt)
            .IsRequired()
            .HasColumnName(nameof(RefreshToken.ExpiresAt).ToSnakeCase());
        
        builder.Property(rt => rt.IsRevoked)
            .HasDefaultValue(false)
            .HasColumnName(nameof(RefreshToken.IsRevoked).ToSnakeCase());
        
        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(45)
            .HasColumnName(nameof(RefreshToken.CreatedByIp).ToSnakeCase());
        
        builder.Property(rt => rt.RevokedAt)
            .HasColumnName(nameof(RefreshToken.RevokedAt).ToSnakeCase());

        builder.Property(rt => rt.RevokedByIp)
            .HasColumnName(nameof(RefreshToken.RevokedByIp).ToSnakeCase());

        builder.Property(rt => rt.UserId)
            .HasColumnName(nameof(RefreshToken.UserId).ToSnakeCase());

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}