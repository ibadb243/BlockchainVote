using Application.CQRS.Commands.Login;
using Application.CQRS.Commands.RefreshToken;
using Application.CQRS.Commands.RegisterUser;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var tokenResponse = await _mediator.Send(new RegisterUserCommand(request.Email, request.Password));
            return Ok(tokenResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var tokenResponse = await _mediator.Send(new LoginCommand(request.Email, request.Password));
            return Ok(tokenResponse);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var tokenResponse = await _mediator.Send(new RefreshTokenCommand(refreshToken));
            return Ok(tokenResponse);
        }
    }
}
