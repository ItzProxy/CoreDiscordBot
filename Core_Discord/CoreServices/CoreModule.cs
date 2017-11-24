using System;
using System.Collections.Generic;
using System.Text;
using NLog;
namespace Core_Discord.CoreServices
{
    public abstract class CoreModules
    {
        public readonly string ModuleTypeName;
        public readonly string LowerModuleTypeName;
        protected readonly Logger _log;

        protected CoreModules()
        {
            //if it's top level module
            ModuleTypeName = this.GetType().DeclaringType.Name;
            LowerModuleTypeName = ModuleTypeName.ToLowerInvariant();
            _log = LogManager.GetCurrentClassLogger();
        }
        public abstract class CoreModule<TService> : CoreModules where TService : CoreService
        {
            public TService service { get; set; }
            public CoreModule() : base()
            {
            }
        }
    }
}
