using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly VoteChainDbContext _context;
        private IDbContextTransaction _currentTransaction;
        private bool _disposed = false;

        private IUserRepository _userRepository;
        private IRefreshTokenRepository _refreshTokenRepository;
        private IPollRepository _pollRepository;
        private IVoteRepository _voteRepository;
        private IBlockRepository _blockRepository;
        private IPendingVoteRepository _pendingVoteRepository;

        public UnitOfWork(VoteChainDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(_context);
        public IPollRepository Polls => _pollRepository ??= new PollRepository(_context);
        public IVoteRepository Votes => _voteRepository ??= new VoteRepository(_context);
        public IBlockRepository Blocks => _blockRepository ??= new BlockRepository(_context);
        public IPendingVoteRepository PendingVotes => _pendingVoteRepository ??= new PendingVoteRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Concurrency conflict occurred", ex);
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                throw new InvalidOperationException("Transaction already started");
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit");
            }

            try
            {
                await SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null) return;

            try
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _currentTransaction?.Dispose();
                _context?.Dispose();
                _disposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                }

                if (_context != null)
                {
                    await _context.DisposeAsync();
                }

                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
