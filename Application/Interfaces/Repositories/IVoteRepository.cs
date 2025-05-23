using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IVoteRepository
    {
        Task<Vote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Vote?> GetByUserAndPollAsync(Guid userId, Guid pollId, CancellationToken cancellationToken = default);
        Task AddAsync(Vote vote, CancellationToken cancellationToken = default);
        Task UpdateAsync(Vote vote, CancellationToken cancellationToken = default);
        Task<List<Vote>> GetByPollAsync(Guid pollId, CancellationToken cancellationToken = default);
    }
}
