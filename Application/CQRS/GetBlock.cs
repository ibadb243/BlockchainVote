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
    public class _dto
    {
        public DateTime timestamp { get; set; }
        public string merkle_root { get; set; }
        public List<BlockTransaction> transactions { get; set; }
        public string previous_hash { get; set; }
        public string hash { get; set; }
        public int nonce { get; set; }
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetBlockRequestHandler(
            IUnitOfWork unitOfWork,
            HybridCache cache,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<_dto>> Handle(GetBlockRequest request, CancellationToken cancellationToken)
        {
            var cachedBlock = await _cache.GetOrCreateAsync($"block-{request.hash}", async token =>
            {
                var block = await _unitOfWork.Blocks.GetByHashAsync(request.hash!, token);
                return block;
            },
                tags: ["block"],
                cancellationToken: cancellationToken);

            if (cachedBlock == null) return Result.NotFound("Block not found");

            return Result.Success(new _dto
            {
                timestamp = cachedBlock.Timestamp,
                transactions = cachedBlock.Transactions,
                merkle_root = cachedBlock.MerkleRoot,
                previous_hash = cachedBlock.PreviousHash,
                hash = cachedBlock.Hash,
                nonce = cachedBlock.Nonce,
            });
        }
    }
}
