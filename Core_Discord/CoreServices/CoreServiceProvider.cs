using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;

namespace Core_Discord.CoreServices
{
    public interface INServiceProvider
    {
    }
    public class CoreServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
