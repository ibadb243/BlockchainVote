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

namespace Application.CQRS.Queries.GetVote
{
    public class GetVoteQueryHandler : IRequestHandler<GetVoteQuery, Result<VoteVerificationDto>>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetVoteQueryHandler(
            IVoteRepository voteRepository,
            HybridCache cache,
            IMapper mapper)
        {
            _voteRepository = voteRepository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<VoteVerificationDto>> Handle(GetVoteQuery request, CancellationToken cancellationToken)
        {
            var cachedVote = await _cache.GetOrCreateAsync($"vote-{request.pollId}-{request.userId}", async token =>
            {
                var vote = await _voteRepository.GetByUserAndPollAsync(request.userId, request.pollId, token);
                return vote;
            },
            tags: ["vote"],
            cancellationToken: cancellationToken);

            if (cachedVote == null) return Result.NotFound("User not found");

            return Result.Success(_mapper.Map<VoteVerificationDto>(cachedVote));
        }
    }
}
