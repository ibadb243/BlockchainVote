using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.CreatePoll
{
    public class CreatePollCommandValidator : AbstractValidator<CreatePollCommand>
    {
        public CreatePollCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Candidates).NotEmpty();
            RuleForEach(x => x.Candidates).ChildRules(candidate =>
            {
                candidate.RuleFor(c => c.Name).NotEmpty();
            });
            RuleFor(x => x.Candidates).Must(c => c.Select(x => x.Name).Distinct().Count() == c.Count).WithMessage("Candidate names must be unique");
            RuleFor(x => x.StartTime).GreaterThan(DateTime.UtcNow);
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
            RuleFor(x => x.MaxSelections)
                .Must((cmd, max) => !max.HasValue || (cmd.IsSurvey && max.Value > 0 && max.Value <= cmd.Candidates.Count))
                .When(x => x.MaxSelections.HasValue)
                .WithMessage("MaxSelections must be positive and not exceed candidate count for survey polls");
        }
    }
}
