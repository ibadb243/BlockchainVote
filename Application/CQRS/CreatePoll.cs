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

namespace Application.CQRS.CreatePoll
{
    public class _dto
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public int candidate_count { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool is_survey { get; set; }
        public bool allow_revote { get; set; }
        public int? max_selection { get; set; }
        public bool is_anonymous { get; set; }
    }

    public class CreatePollRequest : IRequest<Result<_dto>>
    {
        public string? title { get; set; }
        public List<string> candidates { get; set; } = [];
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public bool is_survey { get; set; } = false;
        public bool allow_revote { get; set; } = false;
        public int? max_selection { get; set; }
        public bool is_anonymous { get; set; } = false;
    }

    public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
    {
        public CreatePollRequestValidator()
        {
            RuleFor(x => x.title).NotEmpty().MinimumLength(1).MaximumLength(256);
            RuleFor(x => x.candidates).NotEmpty();
            RuleForEach(x => x.candidates).NotEmpty().MinimumLength(1).MaximumLength(256);
            RuleFor(x => x.start_date).NotEmpty().GreaterThanOrEqualTo(DateTime.UtcNow);
            RuleFor(x => x.end_date).NotEmpty().GreaterThanOrEqualTo(x => x.start_date.Value.AddMinutes(5));
            RuleFor(x => x.max_selection)
                .Must((cmd, max) => !max.HasValue || (cmd.is_survey && max.Value > 0 && max.Value <= cmd.candidates.Count))
                .When(x => x.max_selection.HasValue)
                .WithMessage("max_selection must be positive and not exceed candidate count for survey polls");
        }
    }

    public class CreatePollRequestHandler : IRequestHandler<CreatePollRequest, Result<_dto>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly IUnitOfWork _unitOfÜork;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public CreatePollRequestHandler(
            IPollRepository pollRepository,
            IUnitOfWork unitOfWork,
            HybridCache cache,
            IMapper mapper)
        {
            _pollRepository = pollRepository;
            _unitOfÜork = unitOfWork;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<Result<_dto>> Handle(CreatePollRequest request, CancellationToken cancellationToken)
        {
            var candidates = request.candidates.Select((name, index) => new Candidate
            {
                Id = index + 1,
                Name = name,
                PollId = Guid.Empty
            }).ToList();

            var poll = new Poll
            {
                Id = Guid.NewGuid(),
                Title = request.title!,
                Candidates = candidates,
                StartTime = request.start_date!.Value,
                EndTime = request.end_date!.Value,
                IsSurvey = request.is_survey,
                AllowRevote = request.allow_revote,
                MaxSelections = request.max_selection,
                IsAnonymous = request.is_anonymous,
            };

            foreach (var candidate in poll.Candidates)
            {
                candidate.PollId = poll.Id;
                candidate.Poll = poll;
            }

            await _pollRepository.AddAsync(poll, cancellationToken);
            await _unitOfÜork.SaveChangesAsync(cancellationToken);

            await _cache.SetAsync<Poll>($"poll-{poll.Id}", poll,
                tags: ["poll"],
                cancellationToken: cancellationToken);

            return Result.Created(new _dto
            {
                id = poll.Id,
                title = poll.Title,
                candidate_count = candidates.Count,
                start_date = poll.StartTime,
                end_date = poll.EndTime,
                is_survey = poll.IsSurvey,
                allow_revote = poll.AllowRevote,
                is_anonymous = poll.IsAnonymous,
            });
        }
    }
}
