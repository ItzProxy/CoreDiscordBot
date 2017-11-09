using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreServices.Interfaces
{
    public interface IServiceProvider
    {
        object GetService(Type serviceType);
    }
}
