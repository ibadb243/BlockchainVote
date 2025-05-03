using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetBlock
{
    public class GetBlockQueryHandler : IRequestHandler<GetBlockQuery, BlockDto?>
    {
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;

        public GetBlockQueryHandler(
            IBlockRepository blockRepository,
            IMapper mapper)
        {
            _blockRepository = blockRepository;
            _mapper = mapper;
        }

        public async Task<BlockDto?> Handle(GetBlockQuery request, CancellationToken cancellationToken)
        {
            var block = await _blockRepository.GetByHashAsync(request.Hash);
            return block == null ? null : _mapper.Map<BlockDto>(block);
        }
    }
}
