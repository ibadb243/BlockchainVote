using Application.Interfaces.Repositories;
using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.GetBlockList
{
    public class _dto
    {
        public DateTime timestamp { get; set; }
        public string merkle_root { get; set; }
        public string previous_hash { get; set; }
        public string hash { get; set; }
        public int transaction_count { get; set; }
    }

    public class GetBlockListRequest : IRequest<Result<List<_dto>>>
    {
        public int offset { get; set; } = 0;
        public int limit { get; set; } = 10;
    }

    public class GetBlockListRequestValidator : AbstractValidator<GetBlockListRequest>
    {
        public GetBlockListRequestValidator()
        {
            RuleFor(x => x.offset).GreaterThanOrEqualTo(0);
            RuleFor(x => x.limit).GreaterThanOrEqualTo(1).LessThanOrEqualTo(200);
        }
    }

    public class GetBlockListRequestHandler : IRequestHandler<GetBlockListRequest, Result<List<_dto>>>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;

        public GetBlockListRequestHandler(
            IBlockRepository blockRepository,
            IMapper mapper)
        {
            _blockRepository = blockRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<_dto>>> Handle(GetBlockListRequest request, CancellationToken cancellationToken)
        {
            var blocks = await _blockRepository
                .GetAllQuery()
                .Skip(request.offset)
                .Take(request.limit)
                .Select(block => new _dto
                {
                    timestamp = block.Timestamp,
                    merkle_root = block.MerkleRoot,
                    previous_hash = block.PreviousHash,
                    hash = block.Hash,
                    transaction_count = block.Transactions.Count,
                })
                .ToListAsync(cancellationToken);

            return Result.Success(blocks);
        }
    }
}
