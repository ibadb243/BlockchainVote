using Application.Common.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Users.Commands.CreateCommand;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashHelper _hashHelper;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        IHashHelper hashHelper)
    {
        _context = context;
        _hashHelper = hashHelper;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var FINHash = _hashHelper.CalculateHash(request.FIN);

        var user = await _context.Users.FirstOrDefaultAsync(x => x.FINHash == FINHash, cancellationToken);
        if (user != null) throw new FinIsAlreadyUsedException(request.FIN);

        var passwordHash = _hashHelper.CalculateHash(request.Password);

        user = new User(FINHash, passwordHash);

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}