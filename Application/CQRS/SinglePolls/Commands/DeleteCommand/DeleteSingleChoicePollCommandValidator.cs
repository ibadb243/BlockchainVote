using FluentValidation;

namespace Application.CQRS.SinglePolls.Commands.DeleteCommand;

public class DeleteSingleChoicePollCommandValidator : AbstractValidator<DeleteSingleChoicePollCommand>
{
    public DeleteSingleChoicePollCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PollId).NotEmpty();
    }
}
