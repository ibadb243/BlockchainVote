using FluentValidation;

namespace Application.CQRS.SinglePolls.Commands.CloseCommand;

public class CloseSingleChoicePollCommandValidator : AbstractValidator<CloseSingleChoicePollCommand>
{
    public CloseSingleChoicePollCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PollId).NotEmpty();
    }
}
