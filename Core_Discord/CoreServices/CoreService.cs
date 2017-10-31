using System.Threading.Tasks;

namespace Core_Discord.CoreServices
{
    public interface CoreService
    {
    }

    public interface IUnloadableService
    {
        Task Unload();
    }
}
