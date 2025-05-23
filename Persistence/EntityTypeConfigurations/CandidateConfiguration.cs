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
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder
                .HasKey(x => new { x.PollId, x.Id });

            builder
                .HasIndex(c => new { c.PollId, c.Id })
                .IsUnique();

            builder
                .Property(x => x.Name)
                .IsRequired();
        }
    }
}
