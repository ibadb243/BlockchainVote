using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetBlockList
{
    public class GetBlockListQuery : IRequest<List<BlockDto>>
    {
        public int Offset { get; set; } = 0;
        public int Limit { get; set; } = 10;
    }
}
