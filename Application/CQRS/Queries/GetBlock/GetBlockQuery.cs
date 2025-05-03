using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Queries.GetBlock
{
    public class GetBlockQuery : IRequest<BlockDto?>
    {
        public string Hash{ get; set; }
    }
}
