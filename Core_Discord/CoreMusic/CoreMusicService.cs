using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using Core_Discord.CoreExtensions;
using Core_Discord.CoreDatabase.Models;
using Core_Discord.CoreServices.Interfaces;
using NLog;
using System.IO;
using DSharpPlus.CommandsNext;
using DSharpPlus.Net.WebSocket;
using Google.Apis.Services;
//using NadekoBot.Modules.Music.Common;
//using NadekoBot.Modules.Music.Common.Exceptions;
//using NadekoBot.Modules.Music.Common.SongResolver;
using Core_Discord.CoreServices;

namespace Core_Discord.CoreMusic
{
    public class CoreMusicService : IUnloadableService, CoreService
    {
        public const string MusicPath = "data/music";
        private readonly IGoogleApiService apiService;
        private readonly DbService _db;
        private readonly Logger _log;
        private ICoreCredentials _cred;
        private readonly ConcurrentDictionary<long, float> _defaultVolume;

        public ConcurrentBag<long> GuildDc;

        private readonly DiscordClient _client;

        public ConcurrentDictionary<long, CoreMusicPlayer> MusicPlayers { get; } = new ConcurrentDictionary<long, CoreMusicPlayer>();



        public CoreMusicService(DiscordClient client, DbService db, ICoreCredentials cred, Core core)
        {
            _client = client;
            _db = db;
            _cred = cred;
            _log = LogManager.GetCurrentClassLogger();
        }

        public Task Unload()
        {
            throw new NotImplementedException();
        }
    }
}
