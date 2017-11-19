using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Core_Discord.CoreDatabase.Models;

namespace Core_Discord.CoreDatabase.Repository
{
    public interface IGuildConfigRepository : IRepository<GuildConfig>
    {
        GuildConfig GetOrCreate(long guildId, Func<DbSet<GuildConfig>, IQueryable<GuildConfig>> includes = null);
        ExpSettings ExpSettingsFor(long guildId);
    }
}
