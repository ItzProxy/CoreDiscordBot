using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Core_Discord.CoreDatabase.Models;


namespace Core_Discord.CoreDatabase.Repository
{
    public interface IBotConfigRepository : IRepository<BotConfig>
    {
        BotConfig GetOrCreate(Func<DbSet<BotConfig>, IQueryable<BotConfig>> includes = null);
    }
}
