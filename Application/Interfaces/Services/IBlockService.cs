using Domain.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IBlockService
    {
        Task<Block> CreateBlockAsync(List<BlockTransaction> transactions, string previousHash);
    }
}
