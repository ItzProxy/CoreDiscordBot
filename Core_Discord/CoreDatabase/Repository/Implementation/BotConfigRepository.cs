using Core_Discord.CoreDatabase.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Core_Discord.CoreDatabase;

namespace Core_Discord.CoreDatabase.Repository.Implementation
{
    public class BotConfigRepository : Repository<BotConfig>, IBotConfigRepository
    {
        public BotConfigRepository(DbContext context) : base(context)
        {
        }

        public BotConfig GetOrCreate(Func<DbSet<BotConfig>, IQueryable<BotConfig>> includes = null)
        {
            throw new NotImplementedException();
        }
    }
}


