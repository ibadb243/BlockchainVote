using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class QuickPollConfiguration : IEntityTypeConfiguration<QuickPoll>
{
    public void Configure(EntityTypeBuilder<QuickPoll> builder)
    {
        
    }
}
