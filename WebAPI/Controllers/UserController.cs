using Application.CQRS.Users.Commands.CreateCommand;
using Application.CQRS.Users.Queries.GetDetailsQuery;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers;

[Route("api/user/")]
public class UserController : BaseController
{
    private readonly IMapper _mapper;
    private readonly ITokenHelper _tokenHelper;

    public UserController(
        IMapper mapper,
        ITokenHelper tokenHelper)
    {
        _mapper = mapper;
        _tokenHelper = tokenHelper;
    }

    [AllowAnonymous]
    [HttpPost("createAccount")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var command = _mapper.Map<CreateUserCommand>(request);

        var result = await Mediator.Send(command);

        return Ok(new { access_token = _tokenHelper.GenerateToken(result) });
    }

    [AllowAnonymous]
    [HttpPost("getAccessToken")]
    public async Task<IActionResult> GetAccessToken([FromBody] LoginUserRequest request)
    {
        var command = _mapper.Map<CreateUserCommand>(request);

        var result = await Mediator.Send(command);

        return Ok(new { access_token = _tokenHelper.GenerateToken(result) });
    }

    [Authorize]
    [HttpPost("revokeAccessToken")]
    public async Task<IActionResult> RevokeAccessToken()
    {
        var token = _tokenHelper.GenerateToken(UserId);

        return Ok(new { access_token = token });
    }
}
