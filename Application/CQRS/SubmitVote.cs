﻿using Application.Interfaces.Repositories;
using Ardalis.Result;
using Domain.Blockchain;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.SubmitVote
{
    public class _dto
    {
        public List<int> selection { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class SubmitVoteRequest : IRequest<Result<_dto>>
    {
        public Guid? user { get; set; }
        public Guid? poll { get; set; }
        public List<int> selection { get; set; } = [];
    }

    public class SubmitVoteRequestValidator : AbstractValidator<SubmitVoteRequest>
    {
        public SubmitVoteRequestValidator()
        {
            RuleFor(x => x.user).NotEmpty();
            RuleFor(x => x.poll).NotEmpty();
            RuleFor(x => x.selection).NotEmpty();
        }
    }

    public class SubmitVoteRequestHandler : IRequestHandler<SubmitVoteRequest, Result<_dto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;

        public SubmitVoteRequestHandler(
            IUnitOfWork unitOfWork,
            HybridCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<_dto>> Handle(SubmitVoteRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var cachedUser = await _cache.GetOrCreateAsync($"user-{request.user}", async token =>
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(request.user!.Value, token);
                    return user;
                },
                tags: ["user"],
                cancellationToken: cancellationToken);
                if (cachedUser == null) return Result.NotFound("User not found");

                var cachedPoll = await _cache.GetOrCreateAsync($"poll-{request.poll}", async token =>
                {
                    var poll = await _unitOfWork.Polls.GetByIdAsync(request.poll!.Value, token);
                    return poll;
                },
                tags: ["poll"],
                cancellationToken: cancellationToken);
                if (cachedPoll == null) return Result.NotFound("Poll not found");

                if (DateTime.UtcNow < cachedPoll.StartTime || DateTime.UtcNow > cachedPoll.EndTime)
                    return Result.NotFound("Poll is not active");

                var candidateIds = cachedPoll.Candidates.Select(c => c.Id).ToHashSet();
                if (!request.selection.All(id => candidateIds.Contains(id)))
                    return Result.Conflict("Invalid candidate(s)");

                if (!cachedPoll.IsSurvey && request.selection.Count > 1)
                    return Result.Conflict("Multiple selections not allowed in non-survey poll");

                if (cachedPoll.IsSurvey && cachedPoll.MaxSelections.HasValue && request.selection.Count > cachedPoll.MaxSelections.Value)
                    return Result.Conflict($"Cannot select more than {cachedPoll.MaxSelections.Value} candidates");

                var existingVote = await _unitOfWork.Votes.GetByUserAndPollAsync(request.user!.Value, request.poll!.Value, cancellationToken);

                if (existingVote != null)
                {
                    if (!cachedPoll.AllowRevote)
                        return Result.Conflict("User already voted and revoting is not allowed");

                    existingVote.Timestamp = DateTime.UtcNow;
                    existingVote.Candidates.Clear();
                    existingVote.Candidates.AddRange(request.selection.Select(id => new VoteCandidate
                    {
                        PollId = existingVote.PollId,
                        CandidateId = id,
                    }));

                    await _unitOfWork.Votes.UpdateAsync(existingVote, cancellationToken);
                } else
                {
                    var vote = new Vote
                    {
                        UserId = request.user!.Value,
                        PollId = request.poll!.Value,
                        Timestamp = DateTime.UtcNow,
                        Candidates = request.selection.Select(id => new VoteCandidate
                        {
                            PollId = request.poll!.Value,
                            UserId = request.user!.Value,
                            CandidateId = id,
                        }).ToList()
                    };

                    try
                    {
                        await _unitOfWork.Votes.AddAsync(vote, cancellationToken);
                    }
                    catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                    {
                        return Result.Conflict("Vote already exists");
                    }
                }

                await _unitOfWork.PendingVotes.AddAsync(new PendingVote()
                {
                    UserId = request.user!.Value,
                    PollId = request.poll!.Value,
                    Timestamp = DateTime.UtcNow,
                    CandidateIds = request.selection,
                }, cancellationToken);


                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                await _cache.RemoveAsync($"vote-{request.poll}-{request.user}", cancellationToken);
                await _cache.RemoveAsync($"poll-{request.poll}-result", cancellationToken);

                return Result.Created(new _dto
                {
                    selection = request.selection,
                    timestamp = DateTime.UtcNow,
                });
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException?.Message.Contains("unique constraint") == true ||
                   ex.InnerException?.Message.Contains("duplicate key") == true;
        }
    }
}
