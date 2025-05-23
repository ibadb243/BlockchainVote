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

        public GetPollResultsQueryHandler(
            IVoteRepository voteRepository,
            IPollRepository pollRepository)
        {
            _voteRepository = voteRepository;
            _pollRepository = pollRepository;
        }

        public async Task<Dictionary<int, int>> Handle(GetPollResultsQuery request, CancellationToken cancellationToken)
        {
            var poll = await _pollRepository.GetByIdAsync(request.PollId, cancellationToken);
            if (poll == null)
                throw new Exception("Poll not found");

            var votes = await _voteRepository.GetByPollAsync(request.PollId, cancellationToken);
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

            return results;
        }
    }
}
