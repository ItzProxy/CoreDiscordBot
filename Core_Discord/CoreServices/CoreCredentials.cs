using System;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using Core_Discord.CoreServices.Interfaces;
using System.IO;
using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;
using System.Linq;
using NLog;

namespace Core_Discord.CoreServices
{
    public sealed class CoreCredentials : ICoreCredentials
    {
        //file where credentials are found and made
        private readonly string _credFileName = Path.Combine(Directory.GetCurrentDirectory(), "credentials.json");

        private Logger _log;

        public ulong ClientId { get; }
        public string GoogleApiKey { get; }
        public string SoundCloudClientId { get; }
        public string Token { get; }

        public ImmutableArray<ulong> OwnerIds { get; }

        public RestartConfig RestartCommand { get; }
        public DBConfig Db { get; }
        public int TotalShards { get; }
        public string CarbonKey { get; }

        public string ShardRunCommand { get; }
        public string ShardRunArguments { get; }
        public int ShardRunPort { get; }

        public string PatreonCampaignId { get; }

        public bool UseUserToken { get; set; } = false;

        public CoreCredentials()
        {
            _log = LogManager.GetCurrentClassLogger();

            try
            {
                //create example if it doesn't exists
                File.WriteAllText("./credentials_example.json", JsonConvert.SerializeObject(new CoreCredentialModel(), Formatting.Indented));
            }
            catch { }
            //try if credentials exist
            if (!File.Exists(_credFileName))
            {
                //create file
                //File.Create(Path.Combine(Directory.GetCurrentDirectory(), "credentials.json"));
                File.WriteAllText("./credentials.json", JsonConvert.SerializeObject(new CoreCredentialModel(), Formatting.Indented));
                _log.Warn((LogLevel.Info,
                    typeof(CoreCredentials).ToString(),
                    $"Credentials file is missing, a new one has been generated for you. Please fill it out...there is an example called \n{Path.GetFullPath("./credentials_example.json")}"
                    , DateTime.Now));
                Console.ReadKey(); //cause a block and exit
                return;
            }
            //build config file
            try
            {
                var config = new ConfigurationBuilder();
                config.AddJsonFile(_credFileName, true) //add new file
                .AddEnvironmentVariables("CoreDiscord_");

                var data = config.Build();

                Token = data[nameof(Token)];
                if (string.IsNullOrWhiteSpace(Token))
                {
                    _log.Warn((LogLevel.Warn,
                    typeof(CoreCredentials).ToString(),
                    $"Token is missing, please add it and restart program"
                    ,DateTime.Now));
                    Console.ReadKey(); //cause a block and exit
                    Environment.Exit(3);
                }
                OwnerIds = data.GetSection("OwnerIds").GetChildren().Select(m => ulong.Parse(m.Value)).ToImmutableArray();
                GoogleApiKey = data[nameof(GoogleApiKey)];
                SoundCloudClientId = data[nameof(SoundCloudClientId)];
                ShardRunArguments = data[nameof(ShardRunArguments)];
                ShardRunCommand = data[nameof(ShardRunCommand)];
                UseUserToken = Convert.ToBoolean(data["UseUserToken"]);
                var restartSection = data.GetSection(nameof(RestartCommand));
                var cmd = restartSection["cmd"];
                var args = restartSection["args"];
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    RestartCommand = new RestartConfig(cmd, args);
                }

                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    if (string.IsNullOrWhiteSpace(ShardRunCommand))
                        ShardRunCommand = "dotnet";
                    if (string.IsNullOrWhiteSpace(ShardRunArguments))
                        ShardRunArguments = "run -c Release -- {0} {1}";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(ShardRunCommand))
                        ShardRunCommand = "Core_Discord.exe";
                    if (string.IsNullOrWhiteSpace(ShardRunArguments))
                        ShardRunArguments = "{0} {1}";
                }

                //sets port of process
                var port = data[nameof(ShardRunPort)];
                if (string.IsNullOrWhiteSpace(port))
                {
                    ShardRunPort = new Random().Next(5000, 6000);
                }
                else
                {
                    ShardRunPort = int.Parse(port);
                }

                int ts = 1;
                int.TryParse(data[nameof(TotalShards)], out ts);
                TotalShards = ts < 1 ? 1 : ts;

                ulong.TryParse(data[nameof(ClientId)], out ulong clId);
                ClientId = clId;

                var dbSection = data.GetSection("db");
                Db = new DBConfig(string.IsNullOrWhiteSpace(dbSection["Type"])
                                ? "sql"
                                : dbSection["Type"],
                            string.IsNullOrWhiteSpace(dbSection["ConnectionString"])
                                ? "Data Source=data/CoreDB.db"
                                : dbSection["ConnectionString"]);
            }
            catch (Exception e)
            {
                _log.Fatal((LogLevel.Fatal,
                    nameof(CoreCredentials),
                    e.Message,
                    DateTime.Now));
                throw;
            }
        }

        private class CoreCredentialModel
        {
            public ulong ClientId { get; set; } = 123123123;
            public string Token { get; set; } = "";
            public ulong[] OwnerIds { get; set; } = new ulong[1];
            public string GoogleApiKey { get; set; } = "";
            public string SoundCloudClientId { get; set; } = "";
            public DBConfig Db { get; set; } = new DBConfig("sql", "Data Source=tcp:cs476project.database.windows.net");
            public int TotalShards { get; set; } = 1;
            public string RestartCommand { get; set; } = null;

            public bool UseUserToken { get; set; } = false;
            public string ShardRunCommand { get; set; } = "";
            public string ShardRunArguments { get; set; } = "";
            public int? ShardRunPort { get; set; } = null;
        }

        private class DbModel
        {
            public string Type { get; set; }
            public string ConnectionString { get; set; }
        }

        public bool isOwner(DiscordUser u) => OwnerIds.Contains(u.Id);

        bool ICoreCredentials.IsOwner(DiscordUser u)
        {
            return true;
        }
    }
}
