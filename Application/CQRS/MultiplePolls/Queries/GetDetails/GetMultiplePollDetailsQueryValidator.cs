using FluentValidation;

namespace Application.CQRS.MultiplePolls.Queries.GetDetails;

public class GetMultiplePollDetailsQueryValidator : AbstractValidator<GetMultiplePollDetailsQuery>
{
    public GetMultiplePollDetailsQueryValidator()
    {
        RuleFor(x => x.PollId).NotEmpty();
    }
}
