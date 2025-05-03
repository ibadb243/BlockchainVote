using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetPollResults
{
    public class GetPollResultsQueryHandler : IRequestHandler<GetPollResultsQuery, Dictionary<int, int>>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IPollRepository _pollRepository;
        private readonly ICacheService _cacheService;

        public GetPollResultsQueryHandler(
            IVoteRepository voteRepository,
            IPollRepository pollRepository,
            ICacheService cacheService)
        {
            _voteRepository = voteRepository;
            _pollRepository = pollRepository;
            _cacheService = cacheService;
        }

        public async Task<Dictionary<int, int>> Handle(GetPollResultsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"PollResults_{request.PollId}";
            var cachedResults = await _cacheService.GetAsync<Dictionary<int, int>>(cacheKey);
            if (cachedResults != null)
            {
                return cachedResults;
            }

            var poll = await _pollRepository.GetByIdAsync(request.PollId);
            if (poll == null)
                throw new Exception("Poll not found");

            var votes = await _voteRepository.GetByPollAsync(request.PollId);
            var candidateIds = poll.Candidates.Select(c => c.Id).ToList();
            var results = candidateIds.ToDictionary(id => id, id => 0);

            foreach (var vote in votes)
            {
                foreach (var candidateId in vote.Candidates.Select(c => c.CandidateId))
                {
                    if (results.ContainsKey(candidateId))
                        results[candidateId]++;
                }
            }

            await _cacheService.SetAsync(cacheKey, results);
            return results;
        }
    }
}
