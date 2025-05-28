using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Ardalis.Result;
using Domain.Blockchain;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Application.CQRS.Commands.SubmitVote
{
    public class SubmitVoteCommandHandler : IRequestHandler<SubmitVoteCommand, Result<DateTime>>
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

        public async Task<Result<DateTime>> Handle(SubmitVoteCommand request, CancellationToken cancellationToken)
        {
            var cachedUser = await _cache.GetOrCreateAsync($"user-{request.UserId}", async token =>
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, token);
                return user;
            },
            tags: ["user"],
            cancellationToken: cancellationToken);
            if (cachedUser == null) return Result.NotFound("User not found");

            var cachedPoll = await _cache.GetOrCreateAsync($"poll-{request.PollId}", async token =>
            {
                var poll = await _pollRepository.GetByIdAsync(request.PollId, token);
                return poll;
            },
            tags: ["poll"],
            cancellationToken: cancellationToken);
            if (cachedPoll == null) return Result.NotFound("Poll not found");

            if (DateTime.UtcNow < cachedPoll.StartTime || DateTime.UtcNow > cachedPoll.EndTime)
                return Result.NotFound("Poll is not active");

            var candidateIds = cachedPoll.Candidates.Select(c => c.Id).ToHashSet();
            if (!request.CandidateIds.All(id => candidateIds.Contains(id)))
                return Result.Conflict("Invalid candidate(s)");

            if (!cachedPoll.IsSurvey && request.CandidateIds.Count > 1)
                return Result.Conflict("Multiple selections not allowed in non-survey poll");

            if (cachedPoll.IsSurvey && cachedPoll.MaxSelections.HasValue && request.CandidateIds.Count > cachedPoll.MaxSelections.Value)
                return Result.Conflict($"Cannot select more than {cachedPoll.MaxSelections.Value} candidates");

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
                    return Result.Conflict("User already voted and revoting is not allowed");

                cachedVote.Timestamp = DateTime.UtcNow;
                cachedVote.Candidates.Clear();
                cachedVote.Candidates.AddRange(request.CandidateIds.Select(id => new VoteCandidate
                {
                    PollId = cachedVote.PollId,
                    CandidateId = id,
                }));

                await _voteRepository.UpdateAsync(cachedVote, cancellationToken);
                await _pendingVoteRepository.AddAsync(new PendingVote()
                {
                    UserId = cachedVote.UserId,
                    PollId = cachedVote.PollId,
                    Timestamp = cachedVote.Timestamp,
                    CandidateIds = request.CandidateIds,
                }, cancellationToken);
                return Result.Success(cachedVote.Timestamp);
            }

            var vote = new Vote
            {
                UserId = request.UserId,
                PollId = request.PollId,
                Timestamp = DateTime.UtcNow,
                Candidates = request.CandidateIds.Select(id => new VoteCandidate
                {
                    PollId = Guid.Empty,
                    UserId = Guid.Empty,
                    CandidateId = id,
                }).ToList()
            };

            foreach (var voteCandidate in vote.Candidates)
            {
                voteCandidate.PollId = vote.PollId;
                voteCandidate.UserId = vote.UserId;
            }

            await _voteRepository.AddAsync(vote, cancellationToken);
            await _pendingVoteRepository.AddAsync(new PendingVote() 
            {
                UserId = vote.UserId,
                PollId = vote.PollId,
                Timestamp = vote.Timestamp,
                CandidateIds = request.CandidateIds,
            }, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.SetAsync<Vote>($"vote-{request.PollId}-{request.UserId}", vote,
                tags: ["vote"],
                cancellationToken: cancellationToken);

            return Result.Created(vote.Timestamp);
        }
    }
}
