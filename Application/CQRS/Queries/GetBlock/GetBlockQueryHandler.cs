using Application.Interfaces.Repositories;
using Ardalis.Result;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetBlock
{
    public class GetBlockQueryHandler : IRequestHandler<GetBlockQuery, Result<BlockDto>>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetBlockQueryHandler(
            IBlockRepository blockRepository,
            HybridCache cache,
            IMapper mapper)
        {
            _blockRepository = blockRepository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<BlockDto>> Handle(GetBlockQuery request, CancellationToken cancellationToken)
        {
            var cachedBlock = await _cache.GetOrCreateAsync($"block-{request.Hash}", async token =>
            {
                var block = await _blockRepository.GetByHashAsync(request.Hash, token);
                return block;
            },
            tags: ["block"],
            cancellationToken: cancellationToken);

            if (cachedBlock == null) return Result.NotFound();

            return Result.Success(_mapper.Map<BlockDto>(cachedBlock));
        }
    }
}
