using MediatR;

namespace Application.CQRS.Users.Commands.CreateCommand;

public class CreateUserCommand : IRequest<Guid>
{
    public string FIN { get; set; }
    public string Password { get; set; }
}