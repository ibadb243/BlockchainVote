﻿using Domain.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITransactionConverterService
    {
        BlockTransaction ConvertToBlockTransaction(PendingVote pendingVote);
    }
}
