using FluentValidation;

namespace Application.CQRS.Polls.Commands.CreateCommand.Single;

public class CreateSingleChoicePollCommandValidator : AbstractValidator<CreateSingleChoicePollCommand>
{
    public CreateSingleChoicePollCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).Cascade(CascadeMode.Stop).NotEmpty().MinimumLength(8).MaximumLength(256);
        RuleFor(x => x.Description).MaximumLength(4096);
        RuleFor(x => x.StartDate).Must(s => s.AddMinutes(1) > DateTimeOffset.UtcNow);
        RuleFor(x => x.EndDate).Must((c, e) => e == null || e >= c.StartDate.AddMinutes(5));
        RuleFor(x => x.Options).Must(o => o.Count() > 0 && o.Count() < 32);
        RuleForEach(x => x.Options).SetValidator(new OptionValidator());
    }
}
