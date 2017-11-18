using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core_Discord.CoreDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace Core_Discord.CoreDatabase.Repository.Implementation
{
    public class GuildConfigRepository : Repository<GuildConfig>, IGuildConfigRepository
    {
        public GuildConfigRepository(DbContext context) : base(context)
        {
        }

        public ExpSettings ExpSettingsFor(long guildId)
        {
            var g = GetOrCreate(guildId,
                set => set.Include(x => x.ExpSettings)
                .ThenInclude(x => x.ExpRoleRewardExclusive));

            if(g.ExpSettings == null)
            {
                g.ExpSettings = new ExpSettings();
            }
            return g.ExpSettings;
        }

        public GuildConfig GetOrCreate(long guildId, Func<DbSet<GuildConfig>, IQueryable<GuildConfig>> includes = null)
        {
            GuildConfig config;
            if(includes == null)
            {
                //if I ever get to implementing other guild(server) related fields
                config = _set
                    .FirstOrDefault(c => c.ServerId == guildId);
            }
            else
            {
                var set = includes(_set);
                config = set.FirstOrDefault(c => c.ServerId == guildId);
            }
            if(config == null)
            {
                _set.Add((config = new GuildConfig
                {
                    ServerId = guildId
                }
                ));
                _context.SaveChanges();
            }
            return config;
        }
    }
}
