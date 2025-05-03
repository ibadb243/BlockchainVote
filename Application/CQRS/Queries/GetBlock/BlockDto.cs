using Application.Common.Mappings;
using AutoMapper;
using Domain.Blockchain;

namespace Application.CQRS.Queries.GetBlock
{
    public class BlockDto : IMapWith<Block>
    {
        public DateTime Timestamp { get; set; }
        public string MerkleRoot { get; set; }
        public List<BlockTransaction> Transactions { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public int Nonce { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Block, BlockDto>();
        }
    }
}