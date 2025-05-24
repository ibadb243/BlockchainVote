using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Blockchain;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;

        public SubmitVoteCommandHandler(
            IUserRepository userRepository,
            IPollRepository pollRepository,
            IVoteRepository voteRepository,
            IPendingVoteRepository pendingVoteRepository,
            IUnitOfWork unitOfWork,
            HybridCache cache)
        {
            _userRepository = userRepository;
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
            _pendingVoteRepository = pendingVoteRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Guid> Handle(SubmitVoteCommand request, CancellationToken cancellationToken)
        {
            var cachedUser = await _cache.GetOrCreateAsync($"user-{request.UserId}", async token =>
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, token);
                return user;
            },
            tags: ["user"],
            cancellationToken: cancellationToken);
            if (cachedUser == null) throw new Exception("User not found");

            var cachedPoll = await _cache.GetOrCreateAsync($"poll-{request.PollId}", async token =>
            {
                var poll = await _pollRepository.GetByIdAsync(request.PollId, token);
                return poll;
            },
            tags: ["poll"],
            cancellationToken: cancellationToken);
            if (cachedPoll == null) throw new Exception("Poll not found");

            if (DateTime.UtcNow < cachedPoll.StartTime || DateTime.UtcNow > cachedPoll.EndTime)
                throw new Exception("Poll is not active");

            var candidateIds = cachedPoll.Candidates.Select(c => c.Id).ToHashSet();
            if (!request.CandidateIds.All(id => candidateIds.Contains(id)))
                throw new Exception("Invalid candidate(s)");

            if (!cachedPoll.IsSurvey && request.CandidateIds.Count > 1)
                throw new Exception("Multiple selections not allowed in non-survey poll");

            if (cachedPoll.IsSurvey && cachedPoll.MaxSelections.HasValue && request.CandidateIds.Count > cachedPoll.MaxSelections.Value)
                throw new Exception($"Cannot select more than {cachedPoll.MaxSelections.Value} candidates");

            var cachedVote = await _cache.GetOrCreateAsync($"vote-{request.PollId}-{request.UserId}", async token =>
            {
                var existingVote = await _voteRepository.GetByUserAndPollAsync(request.UserId, request.PollId, cancellationToken);
                return existingVote;
            },
            tags: ["vote"],
            cancellationToken: cancellationToken);

            if (cachedVote != null)
            {
                if (!cachedPoll.AllowRevote)
                    throw new Exception("User already voted and revoting is not allowed");

                cachedVote.Timestamp = DateTime.UtcNow;
                cachedVote.Candidates.Clear();
                cachedVote.Candidates.AddRange(request.CandidateIds.Select(id => new VoteCandidate
                {
                    VoteId = cachedVote.Id,
                    PollId = cachedVote.PollId,
                    CandidateId = id,
                    Vote = cachedVote,
                    Candidate = cachedPoll.Candidates.First(c => c.Id == id)
                }));

                await _voteRepository.UpdateAsync(cachedVote, cancellationToken);
                await _pendingVoteRepository.AddAsync(new PendingVote()
                {
                    VoteId = cachedVote.Id,
                    UserId = cachedVote.UserId,
                    PollId = cachedVote.PollId,
                    Timestamp = cachedVote.Timestamp,
                    CandidateIds = request.CandidateIds,
                }, cancellationToken);
                return cachedVote.Id;
            }

            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PollId = request.PollId,
                Timestamp = DateTime.UtcNow,
                User = cachedUser,
                Poll = cachedPoll,
                Candidates = request.CandidateIds.Select(id => new VoteCandidate
                {
                    VoteId = Guid.Empty,
                    PollId = Guid.Empty,
                    CandidateId = id,
                    Candidate = cachedPoll.Candidates.First(c => c.Id == id)
                }).ToList()
            };

            foreach (var voteCandidate in vote.Candidates)
            {
                voteCandidate.PollId = vote.PollId;
                voteCandidate.VoteId = vote.Id;
                voteCandidate.Vote = vote;
            }

            await _voteRepository.AddAsync(vote, cancellationToken);
            await _pendingVoteRepository.AddAsync(new PendingVote() 
            { 
                VoteId = vote.Id,
                UserId = vote.UserId,
                PollId = vote.PollId,
                Timestamp = vote.Timestamp,
                CandidateIds = request.CandidateIds,
            }, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.SetAsync<Vote>($"vote-{request.PollId}-{request.UserId}", vote,
                tags: ["vote"],
                cancellationToken: cancellationToken);

            return vote.Id;
        }
    }
}
