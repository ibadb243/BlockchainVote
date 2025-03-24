using Domain.Entities;
using Domain.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class VoteConfiguration : IEntityTypeConfiguration<VoteBase>
{
    public void Configure(EntityTypeBuilder<VoteBase> builder)
    {
        builder.HasDiscriminator<string>("VoteType")
            .HasValue<SingleVote>("Single")
            .HasValue<MultipleVote>("Multiple");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();

        builder.HasIndex(x => x.PollId);

        builder.HasIndex(x => x.UserId);
    }
}
