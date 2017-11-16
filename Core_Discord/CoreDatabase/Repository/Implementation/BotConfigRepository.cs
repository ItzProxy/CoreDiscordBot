using Core_Discord.CoreDatabase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;


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

            if (includes == null)
            {
                config = _set
                    .Include(bc => bc.RotatingPlayStatus)
                    .FirstOrDefault();
            }
            else
            {
                config = includes(_set).FirstOrDefault();
            }
            if (config == null)
            {
                _set.Add(config = new BotConfig());
                _context.SaveChanges();
            }
            return config;
        }
    }
}


