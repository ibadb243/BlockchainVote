using Application.Interfaces.Services;
using Domain.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MerkleTreeService : IMerkleTreeService
    {
        public string CalculateMerkleRoot(List<BlockTransaction> transactions)
        {
            if (transactions == null || !transactions.Any())
                return string.Empty;

            var hashes = transactions.Select(t => t.Hash).ToList();
            if (hashes.Count == 1)
                return hashes[0];

            while (hashes.Count > 1)
            {
                var newHashes = new List<string>();
                for (int i = 0; i < hashes.Count; i += 2)
                {
                    var left = hashes[i];
                    var right = i + 1 < hashes.Count ? hashes[i + 1] : left;
                    var combined = left + right;
                    newHashes.Add(ComputeSha256Hash(combined));
                }
                hashes = newHashes;
            }

            return hashes[0];
        }

        private string ComputeSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
