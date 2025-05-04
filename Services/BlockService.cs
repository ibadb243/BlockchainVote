using Application.Interfaces.Services;
using Domain.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class BlockService : IBlockService
    {
        private readonly IMerkleTreeService _merkleTreeService;

        public BlockService(IMerkleTreeService merkleTreeService)
        {
            _merkleTreeService = merkleTreeService;
        }

        public Task<Block> CreateBlockAsync(List<BlockTransaction> transactions, string previousHash)
        {
            string mr = _merkleTreeService.CalculateMerkleRoot(transactions);
            var block = new Block
            {
                Timestamp = DateTime.UtcNow,
                PreviousHash = previousHash,
                Transactions = transactions,
                MerkleRoot = _merkleTreeService.CalculateMerkleRoot(transactions),
                Nonce = 0
            };
            block.Hash = ComputeBlockHash(block);
            return Task.FromResult(block);
        }

        private string ComputeSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLower();
        }

        private string ComputeBlockHash(Block block)
        {
            var data = new
            {
                block.Timestamp,
                block.PreviousHash,
                block.MerkleRoot,
                block.Nonce,
            };
            return ComputeSha256Hash(JsonSerializer.Serialize(data));
        }
    }
}
