using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Blockchain;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.SubmitVote
{
    public class SubmitVoteCommandHandler : IRequestHandler<SubmitVoteCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPollRepository _pollRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly IPendingVoteRepository _pendingVoteRepository;

        public SubmitVoteCommandHandler(
            IUserRepository userRepository,
            IPollRepository pollRepository,
            IVoteRepository voteRepository,
            IPendingVoteRepository pendingVoteRepository)
        {
            _userRepository = userRepository;
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
            _pendingVoteRepository = pendingVoteRepository;
        }

        public async Task<Guid> Handle(SubmitVoteCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new Exception("User not found");

            var poll = await _pollRepository.GetByIdAsync(request.PollId);
            if (poll == null)
                throw new Exception("Poll not found");

            if (DateTime.UtcNow < poll.StartTime || DateTime.UtcNow > poll.EndTime)
                throw new Exception("Poll is not active");

            var candidateIds = poll.Candidates.Select(c => c.Id).ToHashSet();
            if (!request.CandidateIds.All(id => candidateIds.Contains(id)))
                throw new Exception("Invalid candidate(s)");

            if (!poll.IsSurvey && request.CandidateIds.Count > 1)
                throw new Exception("Multiple selections not allowed in non-survey poll");

            if (poll.IsSurvey && poll.MaxSelections.HasValue && request.CandidateIds.Count > poll.MaxSelections.Value)
                throw new Exception($"Cannot select more than {poll.MaxSelections.Value} candidates");

            var existingVote = await _voteRepository.GetByUserAndPollAsync(request.UserId, request.PollId);
            if (existingVote != null)
            {
                if (!poll.AllowRevote)
                    throw new Exception("User already voted and revoting is not allowed");

                existingVote.Timestamp = DateTime.UtcNow;
                existingVote.Candidates.Clear();
                existingVote.Candidates.AddRange(request.CandidateIds.Select(id => new VoteCandidate
                {
                    VoteId = existingVote.Id,
                    PollId = existingVote.PollId,
                    CandidateId = id,
                    Vote = existingVote,
                    Candidate = poll.Candidates.First(c => c.Id == id)
                }));

                await _voteRepository.UpdateAsync(existingVote);
                await _pendingVoteRepository.AddAsync(new PendingVote()
                {
                    VoteId = existingVote.Id,
                    UserId = existingVote.UserId,
                    PollId = existingVote.PollId,
                    Timestamp = existingVote.Timestamp,
                    CandidateIds = request.CandidateIds,
                });
                return existingVote.Id;
            }

            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PollId = request.PollId,
                Timestamp = DateTime.UtcNow,
                User = user,
                Poll = poll,
                Candidates = request.CandidateIds.Select(id => new VoteCandidate
                {
                    VoteId = Guid.Empty,
                    PollId = Guid.Empty,
                    CandidateId = id,
                    Candidate = poll.Candidates.First(c => c.Id == id)
                }).ToList()
            };

            foreach (var voteCandidate in vote.Candidates)
            {
                voteCandidate.PollId = vote.PollId;
                voteCandidate.VoteId = vote.Id;
                voteCandidate.Vote = vote;
            }

            await _voteRepository.AddAsync(vote);
            await _pendingVoteRepository.AddAsync(new PendingVote() 
            { 
                VoteId = vote.Id,
                UserId = vote.UserId,
                PollId = vote.PollId,
                Timestamp = vote.Timestamp,
                CandidateIds = request.CandidateIds,
            });
            return vote.Id;
        }
    }
}
