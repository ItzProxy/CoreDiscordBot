using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Core_Discord.CoreDatabase.Models;

namespace Core_Discord.CoreDatabase.Repository
{
    public interface IGuildConfigRepository : IRepository<GuildConfig>
    {
        GuildConfig GetOrCreate(Func<DbSet<GuildConfig>, IQueryable<GuildConfig>> includes = null);
    }
}
