using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Ardalis.Result;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetPollResults
{
    public class GetPollResultsQueryHandler : IRequestHandler<GetPollResultsQuery, Result<Dictionary<int, int>>>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IPollRepository _pollRepository;
        private readonly HybridCache _cache;

        public GetPollResultsQueryHandler(
            IVoteRepository voteRepository,
            IPollRepository pollRepository,
            HybridCache cache)
        {
            _voteRepository = voteRepository;
            _pollRepository = pollRepository;
            _cache = cache;
        }

        public async Task<Result<Dictionary<int, int>>> Handle(GetPollResultsQuery request, CancellationToken cancellationToken)
        {
            var cachedPoll = await _cache.GetOrCreateAsync($"poll-{request.PollId}", async token =>
            {
                var poll = await _pollRepository.GetByIdAsync(request.PollId, token);
                return poll;
            },
            tags: ["poll"],
            cancellationToken: cancellationToken);
            if (cachedPoll == null) return Result.NotFound("Poll not found");

            var cachedResults = await _cache.GetOrCreateAsync($"poll-{request.PollId}-result", async token =>
            {
                var votes = await _voteRepository.GetByPollAsync(request.PollId, token);
                var candidateIds = cachedPoll.Candidates.Select(c => c.Id).ToList();
                var results = candidateIds.ToDictionary(id => id, id => 0);

                foreach (var vote in votes)
                {
                    foreach (var candidateId in vote.Candidates.Select(c => c.CandidateId))
                    {
                        if (results.ContainsKey(candidateId))
                            results[candidateId]++;
                    }
                }

                return results;
            },
            tags: ["results"],
            cancellationToken: cancellationToken);

            return Result.Success(cachedResults);
        }
    }
}
