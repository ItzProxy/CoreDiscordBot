using System;
using System.Threading.Tasks;

namespace Core_Discord.CoreServices
{
    public interface IStatNextService : CoreService
    {
        string Author { get; }
        long CommandsRan { get; }
        string Heap { get; }
        string Library { get; }
        long MessageCounter { get; }
        double MessagePerSecond { get; }
        long TextChannels { get; }
        long 
            Channels { get; }

        TimeSpan GetUptime();
        string GetUptimeString(string seperator = ", ");
        void Initialize();
        Task<string> Print();
    }
}
