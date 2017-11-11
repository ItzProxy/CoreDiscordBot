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
            BotConfig config;
            if(includes == null)
            {
                config = _set.Include(c => c.RotatingPlayStatus)
                    .FirstOrDefault();
            }
            else
            {
                config = includes(_set).FirstOrDefault();
            }
            if(config == null)
            {
                _set.Add(config = new BotConfig());
                _context.SaveChanges();
            }
            return config;
        }
    }
}


