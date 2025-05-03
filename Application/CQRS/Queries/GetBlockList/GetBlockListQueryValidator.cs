using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetBlockList
{
    public class GetBlockListQueryValidator : AbstractValidator<GetBlockListQuery>
    {
        public GetBlockListQueryValidator()
        {
            RuleFor(x => x.Offset).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Limit).GreaterThan(0).LessThanOrEqualTo(30);
        }
    }
}
