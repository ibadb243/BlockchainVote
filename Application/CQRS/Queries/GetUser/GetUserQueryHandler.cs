using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;
        private readonly HybridCache _cache;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(
            IUserRepository userRepository,
            HybridCache cache,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var cachedUser = await _cache.GetOrCreateAsync($"user-{request.Id}", async token =>
            {
                var user = await _userRepository.GetByIdAsync(request.Id, token);
                return user;
            },
            tags: ["user"],
            cancellationToken: cancellationToken);
            
            return cachedUser == null ? null : _mapper.Map<UserDto>(cachedUser);
        }
    }
}
