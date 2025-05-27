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
    public class PollRepository : IPollRepository
    {
        private readonly VoteChainDbContext _context;

        public PollRepository(VoteChainDbContext context) => _context = context;

        public async Task<Poll?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await _context.Polls
            .Include(p => p.Candidates)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task AddAsync(Poll poll, CancellationToken cancellationToken = default)
        {
            await _context.Polls.AddAsync(poll, cancellationToken);
        }
    }
}
