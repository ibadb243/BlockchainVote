using MediatR;

namespace Application.CQRS.Users.Queries.GetDetailsQuery;

public class GetUserDetailsQuery : IRequest<UserVm>
{
    public Guid UserId { get; set; }
}
