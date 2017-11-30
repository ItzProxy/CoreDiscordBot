using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DSharpPlus;
using Core_Discord.CoreServices.Interfaces;
using NLog;
using System.IO;
using DSharpPlus.VoiceNext;
using DSharpPlus.EventArgs;
using Core_Discord.CoreServices;
using DSharpPlus.Entities;
using Core_Discord.CoreExtensions.ConcurrentCollections;
using static Core_Discord.CoreMusic.CoreMusicExceptions;
using DSharpPlus.CommandsNext;
using Core_Discord.CoreDatabase.Models;
using Core_Discord.CoreMusic.ResolveStrats;
using System.Linq;
using Core_Discord.CoreExtensions;
using DSharpPlus.CommandsNext.Attributes;

namespace Core_Discord.CoreMusic
{
    public class CoreMusicService : IUnloadableService, CoreService
    {
        public const string MusicPath = "data/music";
        private readonly IGoogleApiService _google;
        private readonly DbService _db;
        private readonly Logger _log;
        private ICoreCredentials _cred;
        private readonly ConcurrentDictionary<long, float> _defaultVolumes;
        private readonly object locker = new object();

        public ConcurrentHashSet<long> GuildDc;

        private readonly DiscordClient _client;
        private DiscordClient _discord;
        private CoreCredentials credentials;
        private IGoogleApiService googleApiService;

        public ConcurrentDictionary<long, CoreMusicPlayer> MusicPlayers { get; } = new ConcurrentDictionary<long, CoreMusicPlayer>();



        public CoreMusicService(DiscordClient client,
            DbService db,
            ICoreCredentials cred,
            Core core,
            IGoogleApiService google)
        {
            _google = google;
            _client = client;
            _db = db;
            _cred = cred;
            _log = LogManager.GetCurrentClassLogger();

            _client.GuildDeleted += Discord_GuildDeleted;
            //create music path
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), MusicPath)))
            {
                Directory.CreateDirectory(MusicPath);
            }
        }

        public float GetDefaultVolume(long guildId)
        {
            return _defaultVolumes.GetOrAdd(guildId, (id) =>
            {
                using (var uow = _db.UnitOfWork)
                {
                    return uow.GuildConfig.GetOrCreate(guildId, set => set).DefaultMusicVolume;
                }
            });
        }
        //setup based on user
        public async Task<CoreMusicPlayer> GetOrCreatePlayer(CommandContext e)
        {
            var gUsr = e.User;
            var txtCh = e.Channel;
            var vCh = e.Member?.VoiceState?.Channel;
            //if (vCh == null)
            //{
            //    _log.Warn($"Voice channel not found or {e.User.Username + e.User.Discriminator} is not connected to one");
            //    await e.RespondAsync($"Voice channel not found or {e.User.Username + e.User.Discriminator} is not connected to one").ConfigureAwait(false);
            //    throw new NotInVoiceChannelException();
            //}
            var vnext = e.Client.GetVoiceNext();
            //if (vnext == null){
            //    await e.RespondAsync("Already in guild");
            //}
            //var vnc = await vCh.ConnectAsync().ConfigureAwait(false);
            //await txtCh.SendMessageAsync($"Trying to join `{vCh.Name}` ({vCh.Id})").ConfigureAwait(false);
            return await GetOrCreatePlayer(e.Guild.Id, txtCh, vCh);
        }
        public async Task<CoreMusicPlayer> GetOrCreatePlayer(ulong guild, DiscordChannel textChan, DiscordChannel voiceChan)
        {
            if (voiceChan == null || voiceChan.Guild != textChan.Guild)
            {
                if (textChan != null)
                {
                    await textChan.SendMessageAsync("Not in voice").ConfigureAwait(false);
                }
                throw new NotInVoiceChannelException();
            }
            return MusicPlayers.GetOrAdd((long)guild, _ =>
            {
                float vol = 1.0f;
                var mp = new CoreMusicPlayer(voiceChan, textChan, _google, vol, this);

                DiscordMessage playingMessage = null;
                DiscordMessage lastFinishedMessage = null;
                //add implementation for event trigger
                mp.OnCompleted += async (s, song) =>
                {
                    try
                    {
                        await lastFinishedMessage?.DeleteAsync();
                        try
                        {
                            lastFinishedMessage = await mp.TextChannel.SendMessageAsync(embed: new DiscordEmbedBuilder()
                                .WithColor(DiscordColor.Blue)
                                .WithAuthor("Finished Song", song.FormattedFullName)
                                .WithDescription(song.FormattedName)
                                .WithFooter(song.FormattedInfo))
                                .ConfigureAwait(false);
                        }
                        catch
                        {
                            var cur = mp.Current;
                            if (cur.Current == null
                                && !mp.RepeatCurrentSong
                                && !mp.RepeatPlaylist)
                            {
                                await DestroyPlayer((long)guild).ConfigureAwait(false);
                            }
                        }
                    }
                    catch
                    {

                    }
                };

                mp.OnStarted += async (player, song) =>
                {
                    var sender = player;
                    if (sender == null)
                    {
                        return;
                    }
                    try
                    {
                        await playingMessage?.DeleteAsync();

                        playingMessage = await mp.TextChannel.SendMessageAsync(embed: new DiscordEmbedBuilder().WithColor(DiscordColor.Blue)
                                                    .WithAuthor($"Playing - {song.Index + 1}", song.Song.SongUrl)
                                                    .WithDescription(song.Song.FormattedName)
                                                    .WithFooter($"{mp.Volume} | {song.Song.FormattedInfo}"))
                                                    .ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }
                };
                mp.OnPauseChanged += async (player, paused) =>
                {
                    try
                    {
                        DiscordMessage msg;
                        if (paused)
                        {
                            msg = await mp.TextChannel.SendMessageAsync("paused").ConfigureAwait(false);
                        }
                        else
                        {
                            msg = await mp.TextChannel.SendMessageAsync("resumed").ConfigureAwait(false);
                        }
                        msg?.DeleteAsync();
                    }
                    catch
                    {
                        // ignored
                    }
                };
                _log.Info("Done creating");
                return mp;
            });
        }
        public CoreMusicPlayer GetPlayerOrDefault(long guildId)
        {
            if (MusicPlayers.TryGetValue(guildId, out var mp))
                return mp;
            else
                return null;
        }
        public async Task TryQueueRelatedSongAsync(MusicInfo song, DiscordChannel textChan, DiscordChannel vch)
        {
            var related = (await _google.GetRelatedVideosAsync(song.VideoId, 4)).ToArray();
            if (!related.Any())
                return;

            var mi = await ResolveSong(related[new Random().Next(related.Length)], _client.CurrentUser.ToString(), MusicType.YouTube);
            if (mi == null)
                throw new SongNotFoundException();
            var mp = await GetOrCreatePlayer(textChan.GuildId, vch, textChan);
            mp.Enqueue(mi);
        }
        public async Task<MusicInfo> ResolveSong(string query, string querierName, MusicType? musicType = null)
        {
            query.ThrowIfNull(nameof(query));

            ICoreResolver resolverFactory = new CoreResolver();
            var strategy = await resolverFactory.GetResolveStrategy(query, musicType);
            var sinfo = await strategy.ResolveSong(query);

            if (sinfo == null)
                return null;

            sinfo.QuerierName = querierName;

            return sinfo;
        }
        //event
        private Task Discord_GuildDeleted(GuildDeleteEventArgs e)
        {
            var m = DestroyPlayer((long)e.Guild.Id);
            return Task.CompletedTask;
        }
        /// <summary>
        /// Destory Music Player
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DestroyPlayer(long id)
        {
            _log.Warn("Destorying music player");
            if (MusicPlayers.TryRemove(id, out var mp))
                await mp.Destroy();
        }
        public async Task DestroyAllPlayers()
        {
            foreach (var key in MusicPlayers.Keys)
            {
                await DestroyPlayer(key);
            }
        }
        public Task Unload()
        {
            _client.GuildDeleted -= Discord_GuildDeleted;
            return Task.CompletedTask;
        }
    }
}
