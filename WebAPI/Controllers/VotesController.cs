using Application.CQRS.Commands.SubmitVote;
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
    [Route("api/votes")]
    public class VotesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VotesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitVote([FromBody] SubmitVoteRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Unauthorized"));
            var result = await _mediator.Send(new SubmitVoteCommand(
                userId,
                request.PollId,
                request.CandidateIds
            ));
            return Ok(result);
        }
    }
}
