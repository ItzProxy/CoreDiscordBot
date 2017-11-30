using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using DSharpPlus;
using System;
using Core_Discord.CoreServices;
using Core_Discord.CoreServices.Interfaces;
using static Core_Discord.CoreMusic.CoreMusicExceptions;
using static Core_Discord.CoreServices.CoreModules;
using System.Linq;
using Core_Discord.CoreDatabase.Models;
using NLog;
using System.Collections.Generic;

namespace Core_Discord.CoreMusic
{

    [Group("music")]
    [Description("Contains all music related commands")]
    public class CoreMusicCommands
    {
        private Logger _log;
        private readonly DiscordClient _client;
        private readonly CoreCredentials _creds;
        private IGoogleApiService _google;
        private readonly DbService _db;
        private CoreMusicService _service { get; set; }

        public CoreMusicCommands(DiscordClient client,
            CoreCredentials creds,
            IGoogleApiService google,
            DbService db,
            CoreMusicService mservice)
        {
            _log = LogManager.GetCurrentClassLogger();
            _log.Info("Hello");
            _client = client;
            _creds = creds;
            _google = google;
            _db = db;
            _service = mservice;
        }

        private async Task InternalQueue(CoreMusicPlayer mp, MusicInfo songInfo, bool silent, bool queueFirst = false)
        {
            if (songInfo == null)
            {
                if (!silent)
                    await mp.TextChannel.SendMessageAsync("Song was not found").ConfigureAwait(false);
                return;
            }

            int index;
            try
            {
                index = queueFirst
                    ? mp.EnqueueNext(songInfo)
                    : mp.Enqueue(songInfo);
            }
            catch (QueueFullException)
            {
                await mp.TextChannel.SendMessageAsync("Queue is currently full").ConfigureAwait(false);
                throw;
            }
            if (index != -1)
            {
                if (!silent)
                {
                    try
                    {
                        var embed = new DiscordEmbedBuilder()
                            .WithDescription($"{songInfo.FormattedName}")
                            .WithFooter(songInfo.FormattedProvider);

                        if (Uri.IsWellFormedUriString(songInfo.Thumbnail, UriKind.Absolute))
                            embed.WithThumbnailUrl(songInfo.Thumbnail);
                        int toDisplay = mp.QueueArray().Songs.Length > 10 ? 10 : mp.QueueArray().Songs.Length;
                        for (int i = mp.Current.Index; i < toDisplay; i++)
                        {
                            embed.AddField(i.ToString(), mp.QueueArray().Songs[i].FormattedName);
                        }
                        var queuedMessage = await mp.TextChannel.SendMessageAsync(embed: embed);
                        if (mp.Stopped)
                        {
                            await mp.TextChannel.SendMessageAsync($"Queue Stopped - Use play to add a song to queue").ConfigureAwait(false); 
                        }
                        //await queuedMessage?.DeleteAsync();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
        [Command("queue")]
        [Description("Queue a song")]
        public async Task Queue(CommandContext e, params string[] query)
        {
            var mp = await _service.GetOrCreatePlayer(e);
            var songInfo = await _service.ResolveSong(query[0], e.User.ToString());
            try { await InternalQueue(mp, songInfo, false); } catch (QueueFullException) { return; }
            if (e.Guild.CurrentMember.PermissionsIn(e.Channel).HasPermission(Permissions.ManageMessages)){
                await e.Message.DeleteAsync();
            }
        }
        [Command("next")]
        [Description("Play next song in queue if any")]
        public async Task Next(CommandContext e, int skipCount = 1)
        {
            if (skipCount < 1)
                return;

            var mp = await _service.GetOrCreatePlayer(e);

            mp.Next(skipCount);
        }
        [Command("stop")]
        [Description("Stops current song")]
        public async Task Stop(CommandContext e)
        {
            var mp = await _service.GetOrCreatePlayer(e);
            mp.Stop();
        }
        [Command("pause")]
        [Description("Toggle Pause")]
        public async Task Pause(CommandContext e)
        {
            var mp = await _service.GetOrCreatePlayer(e);
            mp.TogglePause();
        }
        [Command("volume")]
        [Description("Change Volume")]
        public async Task Volume(CommandContext e, float vol)
        {
            var mp = await _service.GetOrCreatePlayer(e);
            if (vol < 0 || vol > 5)
                throw new ArgumentOutOfRangeException(nameof(vol), "Volume needs to be between 0 and 500% inclusive.");

            mp.SetVolume(vol);
            await e.RespondAsync($"Volume set to {(vol * 100).ToString("0.00")}%").ConfigureAwait(false);
        }
        /*
        [Command("save")]
        [Description("Saves current play list to database")]
        public async Task Save(CommandContext e, string name)
        {
            var mp = await _service.GetOrCreatePlayer(e);

            var songs = mp.QueueArray().Songs
                .Select(s => new PlaylistSong()
                {
                    Provider = s.Provider,
                    ProviderType = s.ProviderType,
                    Title = s.Title,
                    Query = s.Query,
                }).ToList();

            PlaylistUser playlist;
            using (var uow = _db.UnitOfWork)
            {
                playlist = new PlaylistUser
                {
                    Name = name,
                    Author = e.User.Username,
                    AuthorId = (long)e.User.Id,
                    Songs = songs.ToList(),
                };
                uow
                await uow.CompleteAsync().ConfigureAwait(false);
            }

            await Context.Channel.EmbedAsync(new EmbedBuilder().WithOkColor()
                .WithTitle(GetText("playlist_saved"))
                .AddField(efb => efb.WithName(GetText("name")).WithValue(name))
                .AddField(efb => efb.WithName(GetText("id")).WithValue(playlist.Id.ToString())));
        }
        */
        [Command("autodelete")]
        [Description("Enables auto delete after song is finished playing")]
        public async Task SongAutoDelete(CommandContext e)
        {
            var mp = await _service.GetOrCreatePlayer(e);
            var val = mp.AutoDelete = !mp.AutoDelete;

            if (val)
            {
                await e.RespondAsync("Auto Delete Enabled").ConfigureAwait(false);
            }
            else
            {
                await e.RespondAsync("Auto Delete Disabled").ConfigureAwait(false);
            }
        }
        [Command("nowplaying")]
        [Description("Displays what is playing")]
        public async Task NowPlaying(CommandContext e)
        {

            var mp = await _service.GetOrCreatePlayer(e);
            var (_, currentSong) = mp.Current;
            if(currentSong == null)
            {
                await e.RespondAsync($"Nothing playing").ConfigureAwait(false);
                return;
            }
            await e.RespondAsync($"Now playing {currentSong.FormattedName}");
        }
        [Command("play")]
        [Description("plays music of the given url")]
        public async Task Play(CommandContext e, string query = null)
        {
            var mp = await _service.GetOrCreatePlayer(e); //get current player
            if (string.IsNullOrWhiteSpace(query)) //check if query is possible
            {
                await Next(e);
            }
            else if (int.TryParse(query, out var index))
                if (index >= 1)
                    mp.SetIndex(index - 1);
                else
                    return;
            else
            {
                try
                {
                    await Queue(e, query);
                }
                catch { }
            }
        }

        [Command("listqueue")]
        [Description("List the contents of the queue")]
        [Aliases("lq")]
        public async Task ShowQueue(CommandContext e, int page = 0)
        {
            var interactivity = e.Client.GetInteractivity();
            var mp = await _service.GetOrCreatePlayer(e);
            var (current, songs) = mp.QueueArray();

            //if queue empty
            if (!songs.Any())
            {
                await e.RespondAsync("Player not available").ConfigureAwait(false);
                return;
            }
            //error checking
            if (--page < -1)
                return;

            try
            {
                await mp.UpdateSongDurationsAsync().ConfigureAwait(false);
            }
            catch { }

            int itemsPage = 5;

            var total = mp.TotalPlayTime;
            var totalStr = total == TimeSpan.MaxValue ? "∞" : $"{(int)(total.TotalHours)}:{total.Minutes}:{total.Seconds}";
            var maxPlaytime = mp.MaxPlaytimeSeconds;

            var mEmbed = new DiscordEmbedBuilder();
            mEmbed.AddField("Current song", songs[current].FormattedFullName);
            for(int i = current + 1; i < songs.Count(); i++)
            {
                mEmbed.AddField("In queue - ", songs[i].FormattedName);
            }
            await e.RespondAsync(embed: mEmbed);
   
        }
    }
}
