using Application.Interfaces.Repositories;
using Domain.Blockchain;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class PendingVoteRepository : IPendingVoteRepository
    {
        private readonly VoteChainDbContext _context;

        public PendingVoteRepository(VoteChainDbContext context) => _context = context;

        public async Task AddAsync(PendingVote vote, CancellationToken cancellationToken = default)
        {
            await _context.PendingVotes.AddAsync(vote, cancellationToken);
        }
            
        public async Task<IList<PendingVote>> GetAllAsync(CancellationToken cancellationToken) => await _context.PendingVotes
            .ToListAsync(cancellationToken);

        public async Task DeleteAllAsync(CancellationToken cancellationToken)
        {
            await _context.PendingVotes.ExecuteDeleteAsync(cancellationToken);
        }

        public Task DeleteRangeAsync(IEnumerable<PendingVote> votes, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.PendingVotes.RemoveRange(votes);
            return Task.CompletedTask;
        }
    }
}
