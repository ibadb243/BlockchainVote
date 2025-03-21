using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class SingleChoicePollConfiguration : IEntityTypeConfiguration<SingleChoicePoll>
{
    public void Configure(EntityTypeBuilder<SingleChoicePoll> builder)
    {
        
    }
}
