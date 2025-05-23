using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Blockchain
{
    public class Block
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string MerkleRoot { get; set; }
        public List<BlockTransaction> Transactions { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public int Nonce { get; set; }

        public readonly static Block GenesisBlock = new Block()
        {
            Timestamp = DateTime.MinValue,
            MerkleRoot = string.Empty,
            Transactions = [],
            PreviousHash = string.Empty,
            Hash = string.Empty,
            Nonce = 0,
        };
    }
}
