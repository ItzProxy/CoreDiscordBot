using System;
using System.Threading.Tasks;
using Core_Discord.CoreDatabase.Repository;
using Core_Discord.CoreDatabase.Repository.Implementation;

namespace Core_Discord.CoreDatabase
{
    public class UnitOfWork : IUnitOfWork
    {
        public CoreContext _context { get; }

        private IBotConfigRepository _botConfig;
        public IBotConfigRepository BotConfig => _botConfig ?? ( _botConfig = new BotConfigRepository(_context));
        
        /// <summary>
        /// constructor gets related database for this bot
        /// </summary>
        /// <param name="context"></param>
        public UnitOfWork(CoreContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Sync save
        /// </summary>
        /// <returns></returns>
        public int Complete() => _context.SaveChanges();
        /// <summary>
        /// Async save 
        /// </summary>
        /// <returns></returns>
        public Task<int> CompleteAsync() => _context.SaveChangesAsync();

        private bool disposed = false;

        protected void Disposed(bool disposin)
        {
            if (!this.disposed)
            {
                if (disposin)
                {
                    _context.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Disposed(true);
            GC.SuppressFinalize(this);
        }

    }
}
