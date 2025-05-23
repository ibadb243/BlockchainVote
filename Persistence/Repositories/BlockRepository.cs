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

        public async Task AddAsync(Block block)
        {
            await _context.AddAsync(block);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Block> GetAllQuery() => _context.Blocks.AsQueryable();

        public async Task<Block?> GetByHashAsync(string hash) => await _context.Blocks.SingleOrDefaultAsync(b => b.Hash == hash);

        public async Task<Block> GetLastAsync() => await _context.Blocks.OrderByDescending(b => b.Id).FirstAsync();
    }
}
