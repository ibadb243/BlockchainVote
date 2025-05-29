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

namespace Application.CQRS.GetPoll
{
    public class _dto
    {
        public string title { get; set; }
        public List<__dto> candidates { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool is_survey { get; set; }
        public bool allow_revote { get; set; }
        public int? max_selection { get; set; }
        public bool is_anonymous { get; set; }

        public class __dto
        {
            public int id { get; set; }
            public string name { get; set; }
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

            return Result.Success(new _dto
            {
                title = cachedPoll.Title,
                candidates = cachedPoll.Candidates.Select(c => new _dto.__dto { id = c.Id, name = c.Name }).ToList(),
                start_date = cachedPoll.StartTime,
                end_date = cachedPoll.EndTime,
                is_survey = cachedPoll.IsSurvey,
                allow_revote = cachedPoll.AllowRevote,
                max_selection = cachedPoll.MaxSelections,
                is_anonymous = cachedPoll.IsAnonymous,
            });
        }
    }
}
