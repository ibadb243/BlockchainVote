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
        Task<Vote?> GetByIdAsync(Guid id);
        Task<Vote?> GetByUserAndPollAsync(Guid userId, Guid pollId);
        Task AddAsync(Vote vote);
        Task UpdateAsync(Vote vote);
        Task<List<Vote>> GetByPollAsync(Guid pollId);
    }
}
