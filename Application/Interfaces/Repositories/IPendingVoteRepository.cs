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
        Task<IList<PendingVote>> GetAllAsync();
        Task AddAsync(PendingVote vote);
        Task DeleteAllAsync();
        Task DeleteRangeAsync(IEnumerable<PendingVote> votes);
    }
}
