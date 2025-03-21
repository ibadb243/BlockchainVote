using FluentValidation;

namespace Application.CQRS.Users.Queries.GetDetailsQuery;

public class GetUserDetailsQueryValidator : AbstractValidator<GetUserDetailsQuery>
{
    public GetUserDetailsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
