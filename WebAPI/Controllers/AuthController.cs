using Application.CQRS.LoginUser;
using Application.CQRS.RefreshToken;
using Application.CQRS.RegisterUser;
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
        public async Task<IActionResult> Register(
            CancellationToken cancellationToken,
            [FromBody] RegisterUserRequest request)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            CancellationToken cancellationToken,
            [FromBody] LoginUserRequest request)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(
            CancellationToken cancellationToken,
            [FromBody] RefreshTokenRequest request)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
