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

        public async Task<Vote?> GetByIdAsync(Guid id)
            => await _context.Votes
                .Include(v => v.Candidates)
                .Include(v => v.Poll)
                .FirstOrDefaultAsync(v => v.Id == id);

        public async Task<Vote?> GetByUserAndPollAsync(Guid userId, Guid pollId)
            => await _context.Votes
                .Include(v => v.Candidates)
                .Include(v => v.Poll)
                .FirstOrDefaultAsync(v => v.UserId == userId && v.PollId == pollId);

        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vote vote)
        {
            _context.Votes.Update(vote);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Vote>> GetByPollAsync(Guid pollId)
            => await _context.Votes
                .Include(v => v.Candidates)
                .Where(v => v.PollId == pollId)
                .ToListAsync();
    }
}
