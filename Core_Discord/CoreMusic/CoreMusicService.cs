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
using DSharpPlus.EventArgs;
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
        private readonly object locker = new object();

        public ConcurrentBag<long> GuildDc;

        private readonly DiscordClient _client;

        public ConcurrentDictionary<long, CoreMusicPlayer> MusicPlayers { get; } = new ConcurrentDictionary<long, CoreMusicPlayer>();



        public CoreMusicService(DiscordClient client, 
            DbService db, 
            ICoreCredentials cred, 
            Core core,
            IGoogleApiService google)
        {
            apiService = google;
            _client = client;
            _db = db;
            _cred = cred;
            _log = LogManager.GetCurrentClassLogger();

            _client.GuildDeleted += Discord_GuildDeleted;
            //create music path
            if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), MusicPath)))
            {
                Directory.CreateDirectory(MusicPath);
            }
            _defaultVolume = new ConcurrentDictionary<long,float>().
        }
        public async Task Stop()
        {

        }
        //event
        private Task Discord_GuildDeleted(GuildDeleteEventArgs e)
        {
            var m = DestroyPlayer();
            return Task.CompletedTask;
        }
        /// <summary>
        /// Destory Music Player
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DestroyPlayer(long id)
        {
            if (MusicPlayers.TryRemove(id, out var mp))
                await mp.Destroy();
        }
        public async Task DestroyPlayer()
        {
            _log.Warn("Destorying music player");
            lock (locker)
            {
                Stop
            }
        }
        public Task Unload()
        {
            _client.GuildDeleted -= Discord_GuildDeleted;
            return Task.CompletedTask;
        }
    }
}
