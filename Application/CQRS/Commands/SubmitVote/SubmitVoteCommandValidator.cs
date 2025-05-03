using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Commands.SubmitVote
{
    public class SubmitVoteCommandValidator : AbstractValidator<SubmitVoteCommand>
    {
        public SubmitVoteCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.PollId).NotEmpty();
            RuleFor(x => x.CandidateIds).NotEmpty();
        }
    }
}
