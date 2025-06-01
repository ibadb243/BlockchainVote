using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly VoteChainDbContext _context;

        public VoteRepository(VoteChainDbContext context) => _context = context;

        public async Task<Vote?> GetByUserAndPollAsync(Guid userId, Guid pollId, CancellationToken cancellationToken = default)
            => await _context.Votes
                .Include(v => v.Candidates)
                .Include(v => v.Poll)
                .FirstOrDefaultAsync(v => v.UserId == userId && v.PollId == pollId, cancellationToken);

        public async Task AddAsync(Vote vote, CancellationToken cancellationToken = default)
        {
            await _context.Votes.AddAsync(vote, cancellationToken);
        }

        public Task UpdateAsync(Vote vote, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _context.Votes.Update(vote);
            return Task.CompletedTask;
        }

        public async Task<List<Vote>> GetByPollAsync(Guid pollId, CancellationToken cancellationToken = default)
            => await _context.Votes
                .Include(v => v.Candidates)
                .Where(v => v.PollId == pollId)
                .ToListAsync(cancellationToken);
    }
}
