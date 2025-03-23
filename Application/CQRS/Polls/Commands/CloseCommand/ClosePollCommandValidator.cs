using FluentValidation;

namespace Application.CQRS.Polls.Commands.CloseCommand;

public class ClosePollCommandValidator : AbstractValidator<ClosePollCommand>
{
    public ClosePollCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PollId).NotEmpty();
    }
}
