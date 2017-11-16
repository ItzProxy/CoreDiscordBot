using System;
using System.Collections.Generic;
using System.Text;
using Core_Discord.CoreDatabase.Repository;
using System.Threading.Tasks;

namespace Core_Discord.CoreDatabase
{
    public interface IUnitOfWork : IDisposable
    {
        CoreContext _context { get; }

        IBotConfigRepository BotConfig { get; } //fi

        int Complete();
        Task<int> CompleteAsync();
    }
}
