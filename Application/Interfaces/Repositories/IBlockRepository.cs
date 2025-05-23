using Domain.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IBlockRepository
    {
        IQueryable<Block> GetAllQuery();
        Task<Block?> GetByHashAsync(string hash, CancellationToken cancellationToken = default);
        Task<Block> GetLastAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Block block, CancellationToken cancellationToken = default);
    }
}
