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

namespace Application.CQRS.Queries.GetPoll
{
    public class GetPollQueryHandler : IRequestHandler<GetPollQuery, Result<PollDto>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetPollQueryHandler(
            IPollRepository pollRepository, 
            HybridCache cache,
            IMapper mapper)
        {
            _pollRepository = pollRepository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<PollDto>> Handle(GetPollQuery request, CancellationToken cancellationToken)
        {
            var cachedPoll = await _cache.GetOrCreateAsync($"poll-{request.Id}", async token =>
            {
                var poll = await _pollRepository.GetByIdAsync(request.Id, token);
                return poll;
            },
            tags: ["poll"],
            cancellationToken: cancellationToken);

            if (cachedPoll == null) return Result.NotFound();

            return Result.Success(_mapper.Map<PollDto>(cachedPoll));
        }
    }
}
