using Domain.Blockchain;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class DbInitializer
    {
        public static void Init(params DbContext[] contexts)
        {
            foreach (var context in contexts)
            {
                context.Database.EnsureCreated();

                if (context is VoteChainDbContext)
                {
                    var c = (VoteChainDbContext)context;

                    if (c.Blocks.Count() == 0)
                    {
                        c.Blocks.Add(Block.GenesisBlock);
                        c.SaveChanges();
                    }
                }
            }
        }
    }
}
