using Domain.Entities;
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
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder
                .HasKey(x => new { x.PollId, x.UserId });

            builder
                .HasIndex(v => new { v.PollId, v.UserId })
                .IsUnique();

            builder
                .HasMany(v => v.Candidates)
                .WithOne(vc => vc.Vote)
                .HasForeignKey(vc => new { vc.PollId, vc.UserId });
        }
    }
}
