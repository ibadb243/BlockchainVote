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

namespace Application.CQRS.GetPoll
{
    public class _dto : IMapWith<Poll>
    {
        public string title { get; set; }
        public List<__dto> candidates { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool is_survey { get; set; }
        public bool allow_revote { get; set; }
        public int? max_selection { get; set; }
        public bool is_anonymous { get; set; }

        public class __dto : IMapWith<Candidate>
        {
            public int id { get; set; }
            public string name { get; set; }

            public void Mapping(Profile profile)
            {
                profile.CreateMap<Candidate, __dto>()
                    .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name));
            }
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Poll, _dto>()
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.candidates, opt => opt.MapFrom(src => src.Candidates))
                .ForMember(dest => dest.start_date, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.end_date, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.is_survey, opt => opt.MapFrom(src => src.IsSurvey))
                .ForMember(dest => dest.allow_revote, opt => opt.MapFrom(src => src.AllowRevote))
                .ForMember(dest => dest.max_selection, opt => opt.MapFrom(src => src.MaxSelections))
                .ForMember(dest => dest.is_anonymous, opt => opt.MapFrom(src => src.IsAnonymous));
        }
    }

    public class GetPollRequest : IRequest<Result<_dto>>
    {
        public Guid? id { get; set; }
    }

    public class GetPollRequestValidator : AbstractValidator<GetPollRequest>
    {
        public GetPollRequestValidator()
        {
            RuleFor(x => x.id).NotEmpty();
        }
    }

    public class GetPollRequestHandler : IRequestHandler<GetPollRequest, Result<_dto>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetPollRequestHandler(
            IPollRepository pollRepository,
            HybridCache cache,
            IMapper mapper)
        {
            _pollRepository = pollRepository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<_dto>> Handle(GetPollRequest request, CancellationToken cancellationToken)
        {
            var cachedPoll = await _cache.GetOrCreateAsync($"poll-{request.id!.Value}", async token =>
            {
                var poll = await _pollRepository.GetByIdAsync(request.id!.Value, token);
                return poll;
            },
            tags: ["poll"],
            cancellationToken: cancellationToken);

            if (cachedPoll == null) return Result.NotFound();

            return Result.Success(_mapper.Map<_dto>(cachedPoll));
        }
    }
}
