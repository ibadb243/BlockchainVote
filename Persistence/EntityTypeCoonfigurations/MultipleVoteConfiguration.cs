using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class MultipleVoteConfiguration : IEntityTypeConfiguration<MultipleVote>
{
    public void Configure(EntityTypeBuilder<MultipleVote> builder)
    {
        
    }
}
