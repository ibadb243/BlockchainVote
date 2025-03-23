using FluentValidation;

namespace Application.CQRS.Polls.Commands.CreateCommand.Multiple;

public class OptionValidator : AbstractValidator<OptionDto>
{
    public OptionValidator()
    {
        RuleFor(x => x.Fullname).NotEmpty().MinimumLength(4).MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(4096);
        RuleFor(x => x.ImagePath).MaximumLength(256);
    }
}
