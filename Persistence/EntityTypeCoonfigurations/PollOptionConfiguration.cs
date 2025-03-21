using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class PollOptionConfiguration : IEntityTypeConfiguration<PollOption>
{
    public void Configure(EntityTypeBuilder<PollOption> builder)
    {
        builder.HasIndex(x => x.PollId);

        builder.Property(x => x.Fullname).HasMaxLength(256).IsRequired();

        builder.Property(x => x.Description).HasMaxLength(4096);

        builder.Property(x => x.ImagePath).HasMaxLength(256);
    }
}
