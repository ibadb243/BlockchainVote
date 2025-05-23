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
    public class PollConfiguration : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasIndex(x => x.Id)
                .IsUnique();

            builder
                .Property(x => x.Title)
                .IsRequired();

            builder
                .Property(x => x.StartTime)
                .IsRequired();

            builder
                .Property(x => x.EndTime)
                .IsRequired();

            builder
                .HasMany(p => p.Candidates)
                .WithOne(c => c.Poll)
                .HasForeignKey(c => c.PollId);

            builder
                .HasMany(p => p.Votes)
                .WithOne(v => v.Poll)
                .HasForeignKey(v => v.PollId);
        }
    }
}
