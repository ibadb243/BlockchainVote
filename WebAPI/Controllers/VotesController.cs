using Application.CQRS.SubmitVote;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> SubmitVote(
            CancellationToken cancellationToken,
            [FromBody] SubmitVoteRequest request)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Unauthorized"));
            request.user = userId;

            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
