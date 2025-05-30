﻿using Application.CQRS.Commands.CreatePoll;
using Application.CQRS.Queries.GetPoll;
using Application.CQRS.Queries.GetPollResults;
using Application.CQRS.Queries.GetVote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/polls")]
    public class PollsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PollsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePoll([FromBody] CreatePollRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Unauthorized"));
            var result = await _mediator.Send(new CreatePollCommand(
                request.Title,
                request.Candidates.Select(dto => new Application.CQRS.Commands.CreatePoll.CreateCandidateDto() { Name = dto.Name }).ToList(),
                request.StartTime ?? DateTime.UtcNow.AddSeconds(1),
                request.EndTime,
                request.IsSurvey,
                request.AllowRevote,
                request.MaxSelections,
                request.IsAnonymous
            ));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPollDetails(Guid id)
        {
            var result = await _mediator.Send(new GetPollQuery(id));
            return Ok(result);
        }

        [HttpGet("{id}/results")]
        public async Task<IActionResult> GetPollResults(Guid id)
        {
            var result = await _mediator.Send(new GetPollResultsQuery(id));
            return Ok(result);
        }

        [HttpGet("{id}/vote")]
        public async Task<IActionResult> GetVote(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Unauthorized"));
            var query = new GetVoteQuery(id, userId);

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
