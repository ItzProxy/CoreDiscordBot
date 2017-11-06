using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using Core_Discord.CoreServices.Interfaces;
using System.IO;
using System.Collections.Immutable;


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

        public ImmutableArray<ulong> OwnerIds { get; }
        public bool IsOwner { get; }
        public int TotalShards { get; }

        public CoreCredentials()
        {
            //try if credentials exist
            if(!File.Exists(_credFileName))
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

            //public string ShardRunCommand { get; set; } = "";
            //public string ShardRunArguments { get; set; } = "";
            //public int? ShardRunPort { get; set; } = null;
        }

        private class DbModel
        {
            public string Type { get; set; }
            public string ConnectionString { get; set; }
        }

        public bool isOwner(DiscordUser u) => OwnerIds.Contains(u.Id);
    }
}
