using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Blockchain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BackgroundServices
{
    public class BlockchainBackgroundService : BackgroundService
    {
        private readonly IPendingVoteRepository _pendingVoteRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly ITransactionConverterService _transactionConverterService;
        private readonly IBlockService _blockService;
        private readonly ILogger<BlockchainBackgroundService> _logger;

        public BlockchainBackgroundService(
            IPendingVoteRepository pendingVoteRepository,
            IBlockRepository blockRepository,
            ITransactionConverterService transactionConverterService,
            IBlockService blockService,
            ILogger<BlockchainBackgroundService> logger)
        {
            _pendingVoteRepository = pendingVoteRepository;
            _blockRepository = blockRepository;
            _transactionConverterService = transactionConverterService;
            _blockService = blockService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10 * 60 * 1000, stoppingToken);

                var votes = await _pendingVoteRepository.GetAllAsync();
                if (votes.Count == 0) continue;

                var lastBlock = await _blockRepository.GetLastAsync();
                var transactions = votes
                    .Select(_transactionConverterService.ConvertToBlockTransaction)
                    .ToList();

                var block = await _blockService.CreateBlockAsync(transactions, lastBlock.Hash);

                await _pendingVoteRepository.DeleteRangeAsync(votes);
                await _blockRepository.AddAsync(block);
            }
        }
    }
}
