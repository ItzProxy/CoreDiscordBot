using Core_Discord.CoreDatabase.Models;
using System.Collections.Generic;


namespace Core_Discord.CoreDatabase.Repository.Implementation
{
    public class BotConfigRepository : Repository<BotConfig>, IBotConfigRepository
    {
        public class BotConfigRepository(DbContext context) : base(context)
    }
}


