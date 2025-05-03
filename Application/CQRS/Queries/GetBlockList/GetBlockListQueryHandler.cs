using Application.Interfaces.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetBlockList
{
    public class GetBlockListQueryHandler : IRequestHandler<GetBlockListQuery, List<BlockDto>>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;

        public GetBlockListQueryHandler(
            IBlockRepository blockRepository,
            IMapper mapper)
        {
            _blockRepository = blockRepository;
            _mapper = mapper;
        }

        public async Task<List<BlockDto>> Handle(GetBlockListQuery request, CancellationToken cancellationToken)
        {
            var blocks = await _blockRepository
                .GetAllQuery()
                .Skip(request.Offset)
                .Take(request.Limit)
                .ProjectTo<BlockDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return blocks;
        }
    }
}
