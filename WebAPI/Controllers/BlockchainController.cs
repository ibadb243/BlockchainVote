using Application.CQRS.Queries.GetBlock;
using Application.CQRS.Queries.GetBlockList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/blockchain")]
    public class BlockchainController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlockchainController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{hash}")]
        public async Task<IActionResult> GetBlock(string hash)
        {
            var query = new GetBlockQuery { Hash = hash };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBlocks([FromQuery] GetBlockListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
