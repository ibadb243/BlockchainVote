using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Block> Blockchain { get; set; }
    public DbSet<PollBase> Polls { get; set; }
    public DbSet<SingleChoicePoll> SingleChoicePolls { get; set; }
    public DbSet<MultipleChoicePoll> MultipleChoicePolls { get; set; }
    public DbSet<QuickPoll> QuickPolls { get; set; }
    public DbSet<VoteBase> Votes { get; set; }
    public DbSet<SingleVote> SingleVotes { get; set; }
    public DbSet<MultipleVote> MultipleVotes { get; set; }
    public DbSet<QuickVote> QuickVotes { get; set; }
    public DbSet<PollOption> PollOption { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }
}
