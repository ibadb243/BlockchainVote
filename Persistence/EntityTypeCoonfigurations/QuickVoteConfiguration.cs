using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class QuickVoteConfiguration : IEntityTypeConfiguration<QuickVote>
{
    public void Configure(EntityTypeBuilder<QuickVote> builder)
    {
        
    }
}
