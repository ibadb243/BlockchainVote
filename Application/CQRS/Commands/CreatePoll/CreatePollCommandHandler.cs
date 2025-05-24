using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.CreatePoll
{
    public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, Guid>
    {
        private readonly IPollRepository _pollRepository;
        private readonly IUnitOfWork _unitOfÜork;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public CreatePollCommandHandler(
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

        public async Task<Guid> Handle(CreatePollCommand request, CancellationToken cancellationToken)
        {
            var candidates = request.Candidates.Select((c, index) => new Candidate
            {
                Id = index + 1,
                Name = c.Name,
                PollId = Guid.Empty
            }).ToList();

            var poll = new Poll
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Candidates = candidates,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsSurvey = request.IsSurvey,
                AllowRevote = request.AllowRevote,
                MaxSelections = request.MaxSelections,
                IsAnonymous = request.IsAnonymous
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

            return poll.Id;
        }
    }
}
