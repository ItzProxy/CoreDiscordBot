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
        public GuildConfig GetOrCreate(Func<DbSet<GuildConfig>, IQueryable<GuildConfig>> includes = null)
        {
            throw new NotImplementedException();
        }
    }
}
