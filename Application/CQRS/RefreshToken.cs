using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Ardalis.Result;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.RefreshToken
{
    public class _dto
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
    }

    public class RefreshTokenRequest : IRequest<Result<_dto>>
    {
        public string? refresh_token { get; set; }
    }

    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.refresh_token).NotEmpty();
        }
    }

    public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, Result<_dto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public RefreshTokenRequestHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<Result<_dto>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (token == null || token.ExpiresAt < DateTime.UtcNow)
                return Result.Unavailable("Invalid or expired refresh token");

            var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
            if (user == null)
                return Result.NotFound("User not found");

            var accessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _refreshTokenRepository.DeleteAsync(user.Id, cancellationToken); // Удаляем старый токен

            var newToken = new Domain.Entities.RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = user
            };

            await _refreshTokenRepository.AddAsync(newToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new _dto
            {
                access_token = accessToken,
                refresh_token = newRefreshToken
            });
        }
    }
}
