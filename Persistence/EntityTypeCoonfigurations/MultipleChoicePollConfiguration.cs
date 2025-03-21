using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityTypeCoonfigurations;

public class MultipleChoicePollConfiguration : IEntityTypeConfiguration<MultipleChoicePoll>
{
    public void Configure(EntityTypeBuilder<MultipleChoicePoll> builder)
    {
        
    }
}
