using Application.CQRS.GetBlock;
using Application.CQRS.GetBlockList;
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
        public async Task<IActionResult> GetBlock(
            CancellationToken cancellationToken,
            [FromQuery] string hash)
        {
            var request = new GetBlockRequest
            {
                hash = hash
            };

            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockList(
            CancellationToken cancellationToken,
            [FromQuery] GetBlockListRequest request)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
