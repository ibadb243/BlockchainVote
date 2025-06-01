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
    public class VoteCandidateConfiguration : IEntityTypeConfiguration<VoteCandidate>
    {
        public void Configure(EntityTypeBuilder<VoteCandidate> builder)
        {
            builder
                .HasKey(vc => new { vc.PollId, vc.UserId, vc.CandidateId });

            builder
               .HasIndex(vc => new { vc.PollId, vc.UserId, vc.CandidateId })
               .IsUnique();

            builder
                .HasOne(vc => vc.Vote)
                .WithMany(v => v.Candidates)
                .HasForeignKey(vc => new { vc.PollId, vc.UserId });

            builder
                .HasOne(vc => vc.Candidate)
                .WithMany(c => c.VoteCandidates)
                .HasForeignKey(vc => new { vc.PollId, vc.CandidateId });
        }
    }
}
