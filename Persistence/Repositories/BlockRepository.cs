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
    public class BlockRepository : IBlockRepository
    {
        private readonly VoteChainDbContext _context;

        public BlockRepository(VoteChainDbContext context) => _context = context;

        public async Task AddAsync(Block block, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(block, cancellationToken);
        }

        public IQueryable<Block> GetAllQuery() => _context.Blocks.AsQueryable();

        public async Task<Block?> GetByHashAsync(string hash, CancellationToken cancellationToken = default) => await _context.Blocks
            .SingleOrDefaultAsync(b => b.Hash == hash, cancellationToken);

        public async Task<Block> GetLastAsync(CancellationToken cancellationToken) => await _context.Blocks
            .OrderByDescending(b => b.Id)
            .FirstAsync(cancellationToken);
    }
}
