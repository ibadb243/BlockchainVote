using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class SingleVoteConfiguration : IEntityTypeConfiguration<SingleVote>
{
    public void Configure(EntityTypeBuilder<SingleVote> builder)
    {
        
    }
}
