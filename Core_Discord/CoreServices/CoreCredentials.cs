using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using Core_Discord.CoreServices.Interfaces;
using System.IO;
using System.Collections.Immutable;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Core_Discord;

namespace Core_Discord.CoreServices
{
    public sealed class CoreCredentials : ICoreCredentials
    {
        //file where credentials are found and made
        private readonly string _credFileName = Path.Combine(Directory.GetCurrentDirectory(), "credentials.json");

        private DebugLogger _log;

        public ulong ClientId { get; }
        public string Token { get; }
        public DBConfig Db { get; }

        public string GoogleApiKey { get; set; } = "";
        public string SoundCloudClientId { get; set; } = "";

        public ImmutableArray<ulong> OwnerIds { get; }
        public bool IsOwner { get; }
        public int TotalShards { get; }

        public string RestartCommand { get; set; } = null;
        public string ShardRunCommand { get; set; } = "";
        public string ShardRunArguments { get; set; } = "";
        public int? ShardRunPort { get; set; } = null;

        RestartConfig ICoreCredentials.RestartCommand { get; }

        public CoreCredentials()
        {
            
            try
            {
                //create example if it doesn't exists
                File.WriteAllText("./credentials.json", JsonConvert.SerializeObject(new CoreCredentialModel(), Formatting.Indented));
            }
            catch { }
            //try if credentials exist
            if (!File.Exists(_credFileName))
            {
                //create file
                File.Create(Path.Combine(Directory.GetCurrentDirectory(), "credentials.json"));
                _log.LogMessage(LogLevel.Info,
                    typeof(CoreCredentials).ToString(),
                    $"Credentials file is missing, a new one has been generated for you. Please fill it out...there is an example called {Path.GetFullPath("./credentials_example.json")}"
                    , DateTime.Now);
                Console.ReadKey(); //cause a block and exit
                return;
            }
            //build config file
            try
            {
                var config = new ConfigurationBuilder();
                config.AddJsonFile(_credFileName, true); //add new file
                //.addenviromentalvariable("CoreDiscord_");

                var data = config.Build();

                if (string.IsNullOrWhiteSpace(Token))
                {
                    _log.LogMessage(LogLevel.Warning,
                    typeof(CoreCredentials).ToString(),
                    $"Token is missing, please add it and restart program"
                    ,DateTime.Now);
                    Console.ReadKey(); //cause a block and exit
                    Environment.Exit(3);
                }
                OwnerIds = data.GetSection("OwnerIds").GetChildren().Select(m => ulong.Parse(m.Value)).ToImmutableArray();
                GoogleApiKey = data[nameof(GoogleApiKey)];
                SoundCloudClientId = data[nameof(SoundCloudClientId)];
                ShardRunArguments = data[nameof(ShardRunArguments)];
                ShardRunCommand = data[nameof(ShardRunCommand)];

                var restartSection = data.GetSection(nameof(RestartCommand));
                var cmd = restartSection["cmd"];

            }
            catch
            {

            }
        }

        private class CoreCredentialModel
        {
            public ulong ClientId { get; set; } = 123123123;
            public string Token { get; set; } = "";
            public ulong[] OwnerIds { get; set; } = new ulong[1];
            public string GoogleApiKey { get; set; } = "";
            public string SoundCloudClientId { get; set; } = "";
            public DBConfig Db { get; set; } = new DBConfig("sql", "Data Source=data/CoreDB.db");
            public int TotalShards { get; set; } = 1;
            public string RestartCommand { get; set; } = null;

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
            throw new NotImplementedException();
        }
    }
}
