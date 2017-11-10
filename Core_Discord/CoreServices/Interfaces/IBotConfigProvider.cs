using Core_Discord.CoreDatabase.Models;
using Core_Discord.CoreCommon;
namespace Core_Discord.CoreServices.Interfaces
{
    public interface IBotConfigProvider
    {
        BotConfig BotConfig { get; }
        void Reload();
        bool Edit(CoreBotConfigEditType type, string newValue);
    }

}
