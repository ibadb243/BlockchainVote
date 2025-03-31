using FluentValidation;

namespace Application.CQRS.QuickPolls.Queries.GetDetails;

public class GetQuickPollDetailsQueryValidator : AbstractValidator<GetQuickPollDetailsQuery>
{
    public GetQuickPollDetailsQueryValidator()
    {
        RuleFor(x => x.PollId).NotEmpty();
    }
}
