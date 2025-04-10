using FluentValidation;

namespace Application.CQRS.SingleVotes.Commands.CreateCommand;

public class CreateSingleVoteCommandValidator : AbstractValidator<CreateSingleVoteCommand>
{
    public CreateSingleVoteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.PollId).NotEmpty();
        RuleFor(x => x.OptionId).NotEqual(0);
    }
}
