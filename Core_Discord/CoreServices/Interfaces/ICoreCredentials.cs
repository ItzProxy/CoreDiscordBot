using DSharpPlus;
using System.Collections.Immutable;


namespace Core_Discord.CoreServices.Interfaces
{
    public interface ICoreCredentials
    {
        ulong ClientId { get; }
        string Token { get; }

        DBConfig Db { get; }
        bool IsOwner { get; }
        int TotalShards { get; }
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
