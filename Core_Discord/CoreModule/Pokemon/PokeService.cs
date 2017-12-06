using Core_Discord.CoreServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core_Discord.CoreModule.Pokemon
{
    public class PokeService : CoreService, IUnloadableService
    {



        public Task Unload()
        {
            throw new NotImplementedException();
        }
    }
}
