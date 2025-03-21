using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

internal class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.Id).IsUnique();

        builder.Property(b => b.VotesJson).IsRequired().HasMaxLength(1024 * 1024);

        builder.Property(b => b.Timestamp).IsRequired();

        builder.Property(b => b.PreviousHash).IsRequired();

        builder.HasIndex(b => b.Hash).IsUnique();
        builder.Property(b => b.Hash).IsRequired();
    }
}
