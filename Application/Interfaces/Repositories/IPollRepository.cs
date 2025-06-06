﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IPollRepository
    {
        Task<Poll?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(Poll poll, CancellationToken cancellationToken = default);
    }
}
