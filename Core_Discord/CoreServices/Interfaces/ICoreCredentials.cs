using DSharpPlus.Entities;
using System.Collections.Immutable;


namespace Core_Discord.CoreServices.Interfaces
{
    public interface ICoreCredentials
    {
        long ClientId { get; }

        string Token { get; }
        string GoogleApiKey { get; }
        ImmutableArray<long> OwnerIds { get; }

        DBConfig Db { get; }

        bool IsOwner(DiscordUser u);
        int TotalShards { get; }
        bool UseUserToken { get; } 
        string ShardRunCommand { get; }
        string ShardRunArguments { get; }
        RestartConfig RestartCommand { get; }
    }

    public class RestartConfig
    {
        public RestartConfig(string cmd, string args)
        {
            this.Cmd = cmd;
            this.Args = args;
        }

        public string Cmd { get; }
        public string Args { get; }
    }

    public class DBConfig
    {
        public DBConfig(string type, string connectionString)
        {
            Type = type;
            ConnectionString = connectionString;
        }
        public string Type { get; }
        public string ConnectionString { get; }
    }
}
