using Application.Common.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Users.Queries.GetDetailsQuery;

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, UserVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserDetailsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserVm> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null) throw new NotFoundException<User>();

        var vm = _mapper.Map<UserVm>(user);

        return vm;
    }
}
