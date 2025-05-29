using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Ardalis.Result;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.RegisterUser
{
    public class _dto
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }

    public class RegisterUserRequest : IRequest<Result<_dto>>
    {
        public string? email { get; set; }
        public string? password { get; set; }
    }

    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.email).NotEmpty().EmailAddress();
            RuleFor(x => x.password).NotEmpty().MinimumLength(6);
        }
    }

    public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, Result<_dto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserRequestHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            HybridCache cache,
            ITokenService tokenService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<_dto>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetByEmailAsync(request.email, cancellationToken) != null)
                return Result.Conflict("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.email,
                PasswordHash = _passwordHasher.HashPassword(request.password)
            };

            await _userRepository.AddAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var token = new Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = user
            };

            await _refreshTokenRepository.AddAsync(token, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _cache.SetAsync($"user-{user.Id}", user,
                tags: ["user"],
                cancellationToken: cancellationToken);

            return Result.Created(new _dto
            {
                access_token = accessToken,
                refresh_token = refreshToken,
            });
        }
    }
}
