using Application.CQRS.Polls.Commands.CloseCommand;
using Application.CQRS.SinglePolls.Commands.CreateCommand;
using Application.CQRS.SinglePolls.Commands.DeleteCommand;
using Application.CQRS.SinglePolls.Queries.GetDetails;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos.CreateSinglePoll;

namespace WebAPI.Controllers;

[Route("api/singlePoll/")]
public class SingleChoicePollController : BaseController
{
    private readonly IMapper _mapper;

    public SingleChoicePollController(IMapper mapper) => _mapper = mapper;

    [Authorize]
    [HttpGet("getPoll")]
    public async Task<IActionResult> Get([FromQuery] Guid poll_id)
    {
        var query = new GetSinglePollDetailsQuery() { PollId = poll_id };

        var result = await Mediator.Send(query);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("createPoll")]
    public async Task<IActionResult> Create([FromBody] CreateSingleChoicePollRequest request)
    {
        var command = _mapper.Map<CreateSingleChoicePollCommand>(request);
        command.UserId = UserId;

        var result = await Mediator.Send(command);

        return Ok(new { poll = result });
    }

    [Authorize]
    [HttpPatch("close")]
    public async Task<IActionResult> Close([FromBody] Guid poll_id)
    {
        var command = new CloseSingleChoicePollCommand() { UserId = UserId, PollId = poll_id };

        await Mediator.Send(command);

        return Ok();
    }

    [Authorize]
    [HttpDelete("deletePoll")]
    public async Task<IActionResult> Delete([FromBody] Guid poll_id)
    {
        var command = new DeleteSingleChoicePollCommand() { UserId = UserId, PollId = poll_id };

        await Mediator.Send(command);

        return Ok();
    }
}
