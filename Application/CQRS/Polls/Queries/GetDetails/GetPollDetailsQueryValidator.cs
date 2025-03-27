using FluentValidation;

namespace Application.CQRS.Polls.Queries.GetDetails;

public class GetPollDetailsQueryValidator : AbstractValidator<GetPollDetailsQuery>
{
    public GetPollDetailsQueryValidator()
    {
        RuleFor(x => x.PollId).NotEmpty();
    }
}
