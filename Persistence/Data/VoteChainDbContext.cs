using Domain.Blockchain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data
{
    public class VoteChainDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<VoteCandidate> VoteCandidates { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<PendingVote> PendingVotes { get; set; }

        public VoteChainDbContext(DbContextOptions<VoteChainDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Votes)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId);

            modelBuilder.Entity<Poll>()
                .HasMany(p => p.Candidates)
                .WithOne(c => c.Poll)
                .HasForeignKey(c => c.PollId);

            modelBuilder.Entity<Poll>()
                .HasMany(p => p.Votes)
                .WithOne(v => v.Poll)
                .HasForeignKey(v => v.PollId);

            modelBuilder.Entity<Vote>()
                .HasIndex(v => new { v.UserId, v.PollId })
                .IsUnique();

            modelBuilder.Entity<Candidate>()
                .HasIndex(c => new { c.PollId, c.Id })
                .IsUnique();

            modelBuilder.Entity<VoteCandidate>()
                .HasKey(vc => new { vc.VoteId, vc.CandidateId });

            modelBuilder.Entity<VoteCandidate>()
                .HasOne(vc => vc.Vote)
                .WithMany(v => v.Candidates)
                .HasForeignKey(vc => vc.VoteId);

            modelBuilder.Entity<VoteCandidate>()
                .HasOne(vc => vc.Candidate)
                .WithMany(c => c.VoteCandidates)
                .HasForeignKey(vc => vc.CandidateId);

            modelBuilder.Entity<VoteCandidate>()
                .HasIndex(vc => vc.VoteId);

            modelBuilder.Entity<VoteCandidate>()
                .HasIndex(vc => vc.CandidateId);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(t => t.Token)
                .IsUnique();

            modelBuilder.Entity<Block>()
                .OwnsMany(b => b.Transactions, builder => builder.ToJson());

            modelBuilder.Entity<Block>()
                .HasKey(b => b.Hash);

            modelBuilder.Entity<Block>()
                .HasIndex(b => b.Hash);

            modelBuilder.Entity<PendingVote>()
                .HasNoKey();
        }
    }
}
