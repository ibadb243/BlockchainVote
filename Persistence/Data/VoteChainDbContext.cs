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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoteChainDbContext).Assembly);
        }
    }
}
