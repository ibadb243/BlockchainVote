using FluentValidation;

namespace Application.CQRS.SinglePolls.Queries.GetDetails;

public class GetSinglePollDetailsQueryValidator : AbstractValidator<GetSinglePollDetailsQuery>
{
    public GetSinglePollDetailsQueryValidator()
    {
        RuleFor(x => x.PollId).NotEmpty();
    }
}
