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

        public async Task AddAsync(PendingVote vote)
        {
            await _context.PendingVotes.AddAsync(vote);
            await _context.SaveChangesAsync();
        }
            
        public async Task<IList<PendingVote>> GetAllAsync() => await _context.PendingVotes.ToListAsync();

        public async Task DeleteAllAsync()
        {
            await _context.PendingVotes.ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<PendingVote> votes)
        {
            _context.PendingVotes.RemoveRange(votes);
            await _context.SaveChangesAsync();
        }
    }
}
