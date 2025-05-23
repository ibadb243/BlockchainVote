using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetPoll
{
    public class GetPollQueryHandler : IRequestHandler<GetPollQuery, PollDto?>
    {
        private readonly IPollRepository _pollRepository;
        private readonly IMapper _mapper;

        public GetPollQueryHandler(IPollRepository pollRepository, IMapper mapper)
        {
            _pollRepository = pollRepository;
            _mapper = mapper;
        }

        public async Task<PollDto?> Handle(GetPollQuery request, CancellationToken cancellationToken)
        {
            var poll = await _pollRepository.GetByIdAsync(request.Id, cancellationToken);
            return poll == null ? null : _mapper.Map<PollDto>(poll);
        }
    }
}
