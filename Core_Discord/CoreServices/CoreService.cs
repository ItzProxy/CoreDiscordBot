using System.Threading.Tasks;

namespace Core_Discord.CoreServices
{
    /// <summary>
    /// To be able to picked up as service
    /// </summary>
    public interface CoreService
    {
    }
    /// <summary>
    /// Unload services for clean up
    /// </summary>
    public interface IUnloadableService
    {
        Task Unload();
    }
}
