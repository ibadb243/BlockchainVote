using Domain.Entities;
using Domain.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class PollConfiguration : IEntityTypeConfiguration<PollBase>
{
    public void Configure(EntityTypeBuilder<PollBase> builder)
    {
        builder.HasDiscriminator<string>("PollType")
            .HasValue<SingleChoicePoll>("Single")
            .HasValue<MultipleChoicePoll>("Multiple")
            .HasValue<QuickPoll>("Quick");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.Title).HasMaxLength(256).IsRequired();

        builder.Property(x =>x.Description).HasMaxLength(4096);
    }
}
