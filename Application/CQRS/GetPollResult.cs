using Application.Interfaces.Repositories;
using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.GetPollResult
{
    public class _dto
    {
        public int vote_count { get; set; }
        public Dictionary<int, int> results { get; set; }
    }

    public class GetPollResultRequest : IRequest<Result<_dto>>
    {
        public Guid? id { get; set; }
    }

    public class GetPollResultRequestValidator : AbstractValidator<GetPollResultRequest>
    {
        public GetPollResultRequestValidator()
        {
            RuleFor(x => x.id).NotEmpty();
        }
    }

    public class GetPollResultRequestHandler : IRequestHandler<GetPollResultRequest, Result<_dto>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly HybridCache _cache;

        public GetPollResultRequestHandler(
            IPollRepository pollRepository,
            IVoteRepository voteRepository,
            HybridCache cache)
        {
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
            _cache = cache;
        }

        public async Task<Result<_dto>> Handle(GetPollResultRequest request, CancellationToken cancellationToken)
        {
            var cachedPoll = await _cache.GetOrCreateAsync($"poll-{request.id}", async token =>
            {
                var poll = await _pollRepository.GetByIdAsync(request.id!.Value, token);
                return poll;
            },
            tags: ["poll"],
            cancellationToken: cancellationToken);
            if (cachedPoll == null) return Result.NotFound("Poll not found");

            var cachedResults = await _cache.GetOrCreateAsync($"poll-{request.id}-result", async token =>
            {
                var votes = await _voteRepository.GetByPollAsync(request.id!.Value, token);
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

            return Result.Success(new _dto {
                vote_count = cachedPoll.Votes.Count,
                results = cachedResults
            });
        }
    }
}
