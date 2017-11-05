using System;
using System.Collections.Generic;
using System.Text;
using Core_Discord.CoreDatabase.Repository;

namespace Core_Discord.CoreDatabase
{
    public interface IUnitOfWork
    {
        CoreContext _context { get; }

        IBotConfigRepository BotConfig { get; }

    }
}
