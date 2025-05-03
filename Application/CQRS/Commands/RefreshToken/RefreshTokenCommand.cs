using Application.CQRS.Commands.Login;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponse>;
}