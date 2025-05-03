using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetBlock
{
    public class GetBlockQueryValidator : AbstractValidator<GetBlockQuery>
    {
        public GetBlockQueryValidator()
        {
            RuleFor(x => x.Hash).NotEmpty();
        }
    }
}
