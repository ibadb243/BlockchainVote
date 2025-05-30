﻿using Ardalis.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.RegisterUser
{
    public record RegisterUserCommand(
        string Email, 
        string Password) 
        : IRequest<Result<TokenResponse>>;
}
