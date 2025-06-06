﻿using Application.CQRS.CreatePoll;
using Application.CQRS.GetPoll;
using Application.CQRS.GetPollResult;
using Application.CQRS.GetVote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> CreatePoll(
            CancellationToken cancellationToken,
            [FromBody] CreatePollRequest request)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{poll_id:guid}")]
        public async Task<IActionResult> GetPollDetails(
            CancellationToken cancellationToken,
            [FromRoute] Guid poll_id)
        {
            var request = new GetPollRequest { id = poll_id };

            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{poll_id:guid}/results")]
        public async Task<IActionResult> GetPollResults(
            CancellationToken cancellationToken,
            [FromRoute] Guid poll_id)
        {
            var request = new GetPollResultRequest { id = poll_id };

            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{poll_id:guid}/vote")]
        public async Task<IActionResult> GetVote(
            CancellationToken cancellationToken,
            [FromRoute] Guid poll_id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Unauthorized"));

            var request = new GetVoteRequest
            {
                poll_id = poll_id,
                user_id = userId
            };

            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
