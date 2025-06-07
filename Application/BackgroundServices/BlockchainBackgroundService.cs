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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionConverterService _transactionConverterService;
        private readonly IBlockService _blockService;
        private readonly ILogger<BlockchainBackgroundService> _logger;

        public BlockchainBackgroundService(
            IUnitOfWork unitOfWork,
            ITransactionConverterService transactionConverterService,
            IBlockService blockService,
            ILogger<BlockchainBackgroundService> logger)
        {
            _unitOfWork = unitOfWork;
            _transactionConverterService = transactionConverterService;
            _blockService = blockService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _unitOfWork.RollbackTransactionAsync(stoppingToken);
                await Task.Delay(10 * 60 * 1000, stoppingToken);

                await _unitOfWork.BeginTransactionAsync(stoppingToken);

                try
                {
                    var votes = await _unitOfWork.PendingVotes.GetAllAsync();
                    if (votes.Count == 0) continue;

                    var lastBlock = await _unitOfWork.Blocks.GetLastAsync();
                    var transactions = votes
                        .Select(_transactionConverterService.ConvertToBlockTransaction)
                        .ToList();

                    var block = await _blockService.CreateBlockAsync(transactions, lastBlock.Hash);

                    await _unitOfWork.PendingVotes.DeleteRangeAsync(votes);
                    await _unitOfWork.Blocks.AddAsync(block);
                    await _unitOfWork.SaveChangesAsync(stoppingToken);

                    await _unitOfWork.CommitTransactionAsync(stoppingToken);
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync(stoppingToken);
                    throw;
                }
            }
        }
    }
}
