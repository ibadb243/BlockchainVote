using Application.Common.Mappings;
using Application.Interfaces.Repositories;
using Ardalis.Result;
using AutoMapper;
using Domain.Entities;
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
    public class _dto : IMapWith<Vote>
    {
        public Guid? user_id { get; set; }
        public Guid poll_id { get; set; }
        public List<int> selection { get; set; }
        public DateTime timestamp { get; set; }
        public bool is_anonymous { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<VoteCandidate, int>().ConvertUsing(src => src.CandidateId);

            profile.CreateMap<Vote, _dto>()
                .ForMember(dest => dest.user_id, opt => opt.Condition(src => src.Poll.IsAnonymous == false))
                .ForMember(dest => dest.poll_id, opt => opt.MapFrom(src => src.PollId))
                .ForMember(dest => dest.selection, opt => opt.MapFrom(src => src.Candidates))
                .ForMember(dest => dest.timestamp, opt => opt.MapFrom(src => src.Timestamp))
                .ForMember(dest => dest.is_anonymous, opt => opt.MapFrom(src => src.Poll.IsAnonymous));
        }
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
        private readonly IPollRepository _pollRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetVoteRequestHandler(
            IPollRepository pollRepository,
            IVoteRepository voteRepository,
            HybridCache cache,
            IMapper mapper)
        {
            _pollRepository = pollRepository;
            _voteRepository = voteRepository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<_dto>> Handle(GetVoteRequest request, CancellationToken cancellationToken)
        {
            var cachedVote = await _cache.GetOrCreateAsync($"vote-{request.poll_id}-{request.user_id}", async token =>
            {
                var vote = await _voteRepository.GetByUserAndPollAsync(request.user_id!.Value, request.poll_id!.Value, token);
                return vote;
            },
            tags: ["vote"],
            cancellationToken: cancellationToken);

            if (cachedVote == null) return Result.NotFound("Vote not found");

            var cachedPoll = await _cache.GetOrCreateAsync($"poll-{cachedVote.PollId}", async token =>
            {
                var poll = await _pollRepository.GetByIdAsync(cachedVote.PollId, token);
                return poll;
            },
            tags: ["poll"],
            cancellationToken: cancellationToken);

            if (cachedPoll == null) return Result.Error("Invalid vote");

            return Result.Success(_mapper.Map<_dto>(cachedVote));
        }
    }
}
