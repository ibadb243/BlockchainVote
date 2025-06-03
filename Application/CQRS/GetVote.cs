using Application.Interfaces.Repositories;
using Ardalis.Result;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.GetVote
{
    public class _dto
    {
        public Guid? user_id { get; set; }
        public Guid poll_id { get; set; }
        public List<int> selection { get; set; }
        public DateTime timestamp { get; set; }
        public bool is_anonymous { get; set; }
    }

    public class GetVoteRequest : IRequest<Result<_dto>>
    {
        public Guid? poll_id { get; set; }
        public Guid? user_id { get; set; }
    }

    public class GetVoteRequestValidator : AbstractValidator<GetVoteRequest>
    {
        public GetVoteRequestValidator()
        {
            RuleFor(x => x.poll_id).NotEmpty();
            RuleFor(x => x.user_id).NotEmpty();
        }
    }

    public class GetVoteRequestHandler : IRequestHandler<GetVoteRequest, Result<_dto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetVoteRequestHandler(
            IUnitOfWork unitOfWork,
            HybridCache cache,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<_dto>> Handle(GetVoteRequest request, CancellationToken cancellationToken)
        {
            var cachedVote = await _cache.GetOrCreateAsync($"vote-{request.poll_id}-{request.user_id}", async token =>
            {
                var vote = await _unitOfWork.Votes.GetByUserAndPollAsync(request.user_id!.Value, request.poll_id!.Value, token);
                return vote;
            },
            tags: ["vote"],
            cancellationToken: cancellationToken);

            if (cachedVote == null) return Result.NotFound("Vote not found");

            var cachedPoll = await _cache.GetOrCreateAsync($"poll-{cachedVote.PollId}", async token =>
            {
                var poll = await _unitOfWork.Polls.GetByIdAsync(cachedVote.PollId, token);
                return poll;
            },
            tags: ["poll"],
            cancellationToken: cancellationToken);

            if (cachedPoll == null) return Result.Error("Invalid vote");

            return Result.Success(new _dto
            {
                poll_id = cachedVote.PollId,
                user_id = cachedPoll.IsAnonymous ? null : cachedVote.UserId,
                selection = cachedVote.Candidates.Select(vc => vc.CandidateId).ToList(),
                timestamp = cachedVote.Timestamp,
                is_anonymous = cachedPoll.IsAnonymous,
            });
        }
    }
}
