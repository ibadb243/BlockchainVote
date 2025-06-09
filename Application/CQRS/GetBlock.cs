using Application.Common.Mappings;
using Application.Interfaces.Repositories;
using Ardalis.Result;
using AutoMapper;
using Domain.Blockchain;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.GetBlock
{
    public class _dto : IMapWith<Block>
    {
        public DateTime timestamp { get; set; }
        public string merkle_root { get; set; }
        public List<BlockTransaction> transactions { get; set; }
        public string previous_hash { get; set; }
        public string hash { get; set; }
        public int nonce { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Block, _dto>()
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.merkle_root, opt => opt.MapFrom(src => src.MerkleRoot))
                .ForMember(dest => dest.transactions, opt => opt.MapFrom(src => src.Transactions))
                .ForMember(dest => dest.previous_hash, opt => opt.MapFrom(src => src.PreviousHash))
                .ForMember(dest => dest.hash, opt => opt.MapFrom(src => src.Hash))
                .ForMember(dest => dest.nonce, opt => opt.MapFrom(src => src.Nonce));
        }
    }

    public class GetBlockRequest : IRequest<Result<_dto>>
    {
        public string? hash { get; set; }
    }

    public class GetBlockRequestValidator : AbstractValidator<GetBlockRequest>
    {
        public GetBlockRequestValidator()
        {
            RuleFor(x => x.hash).NotEmpty();
        }
    }

    public class GetBlockRequestHandler : IRequestHandler<GetBlockRequest, Result<_dto>>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetBlockRequestHandler(
            IBlockRepository blockRepository,
            HybridCache cache,
            IMapper mapper)
        {
            _blockRepository = blockRepository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<_dto>> Handle(GetBlockRequest request, CancellationToken cancellationToken)
        {
            var cachedBlock = await _cache.GetOrCreateAsync($"block-{request.hash}", async token =>
            {
                var block = await _blockRepository.GetByHashAsync(request.hash!, token);
                return block;
            },
                tags: ["block"],
                cancellationToken: cancellationToken);

            if (cachedBlock == null) return Result.NotFound("Block not found");

            return Result.Success(_mapper.Map<_dto>(cachedBlock));
        }
    }
}
