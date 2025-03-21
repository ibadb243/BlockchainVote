using Domain.Entities;
using Domain.Entities.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<PollBase> Polls { get; }
    DbSet<SingleChoicePoll> SingleChoicePolls { get; }
    DbSet<MultipleChoicePoll> MultipleChoicePolls { get; }
    DbSet<QuickPoll> QuickPolls { get; }
    DbSet<PollOption> PollOption { get; }
    DbSet<Block> Blockchain { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}