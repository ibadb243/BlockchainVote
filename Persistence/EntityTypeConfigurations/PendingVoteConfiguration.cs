using Domain.Blockchain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityTypeConfigurations
{
    public class PendingVoteConfiguration : IEntityTypeConfiguration<PendingVote>
    {
        public void Configure(EntityTypeBuilder<PendingVote> builder)
        {
            builder
                .HasKey(pv => pv.Id);

            builder
                .Property(pv => pv.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
