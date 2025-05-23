using Domain.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IPendingVoteRepository
    {
        Task<IList<PendingVote>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(PendingVote vote, CancellationToken cancellationToken = default);
        Task DeleteAllAsync(CancellationToken cancellationToken = default);
        Task DeleteRangeAsync(IEnumerable<PendingVote> votes, CancellationToken cancellationToken = default);
    }
}
