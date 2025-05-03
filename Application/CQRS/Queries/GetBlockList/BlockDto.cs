using Application.Common.Mappings;
using AutoMapper;
using Domain.Blockchain;

namespace Application.CQRS.Queries.GetBlockList
{
    public class BlockDto : IMapWith<Block>
    {
        public DateTime Timestamp { get; set; }
        public string MerkleRoot { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public int CountTransactions { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Block, BlockDto>()
                .ForMember(dest => dest.CountTransactions, opt => opt.MapFrom(src => src.Transactions.Count()));
        }
    }
}