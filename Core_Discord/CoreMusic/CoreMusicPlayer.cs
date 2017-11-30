using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;
using DSharpPlus.Exceptions;
using DSharpPlus;
using System.Linq;
using DSharpPlus.Entities;
using NLog;
using Core_Discord.CoreServices.Interfaces;
using Core_Discord.Music;
using Core_Discord.CoreDatabase.Models;
using System.Diagnostics;
using System.IO;
using DSharpPlus.CommandsNext;

/// <summary>
/// Built using an Outdated Music Player for Discord
/// Retooled to help play music through voice channel
/// </summary>

namespace Core_Discord.CoreMusic
{
    public enum StreamState
    {
        Resolving,
        Queued,
        Playing,
        Completed
    }
    public partial class CoreMusicPlayer : IDisposable
    {
        private Logger _log;
        private readonly object locker = new object(); //semaphore
        private readonly Thread _player;
        public DiscordChannel VoiceChannel { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public DiscordChannel TextChannel { get; set; }
        private readonly IGoogleApiService _google;
        private CoreMusicService _musicService;
        //logger

        private CoreMusicQueue Queue { get; } = new CoreMusicQueue();
        private VoiceNextConnection _audioClient;
        private TaskCompletionSource<bool> pauseTaskSource { get; set; } = null;
        public bool Exited { get; set; } = false;
        public bool Stopped { get; private set; } = false;
        public float Volume { get; private set; } = 1.0f;
        public bool Paused => pauseTaskSource != null;

        public string VolumeIcon => $"🔉 {(int)(Volume * 100)}%";
        public TimeSpan CurrentTime => TimeSpan.FromSeconds(_bytesSent / (float)_frameBytes / (1000 / _miliseconds));
        public string FormattedCurrentTime
        {
            get
            {
                var time = CurrentTime.ToString(@"mm\:ss");
                var hrs = (int)CurrentTime.TotalHours;

                if (hrs > 0)
                {
                    return hrs + ":" + time;
                }
                else
                {
                    return time;
                }
            }
        }
        //time related variables
        private int _bytesSent = 0;
        const float _miliseconds = 20.0f;
        const int _frameBytes = 3840;

        #region events
        public event Action<CoreMusicPlayer, (int Index, MusicInfo Song)> OnStarted;
        public event Action<CoreMusicPlayer, MusicInfo> OnCompleted;
        public event Action<CoreMusicPlayer, bool> OnPauseChanged;
        #endregion

        private bool manualSkip = false;
        private bool manualIndex = false;
        private bool newVoiceChannel = false;

        private bool cancel = false;

        public TimeSpan TotalPlayTime
        {
            get
            {
                var songs = Queue.ToArray().Songs;
                return songs.Any(x => x.TotalTime == TimeSpan.MaxValue)
                    ? TimeSpan.MaxValue
                    : new TimeSpan(songs.Sum(s => s.TotalTime.Ticks));
            }
        }
        //More player options
        public bool AutoDelete { get; set; }
        public uint MaxPlaytimeSeconds { get; set; }
        public bool RepeatCurrentSong { get; private set; }
        public bool Shuffle { get; private set; }
        public bool Autoplay { get; private set; }
        public bool RepeatPlaylist { get; private set; } = false;
        public uint MaxQueueSize
        {
            get => Queue.MaxQueueSize;
            set { lock (locker) Queue.MaxQueueSize = value; }
        }
        /// <summary>
        /// getter for <int,MusicInfo>
        /// </summary>
        public (int Index, MusicInfo Current) Current
        {
            get
            {
                if (Stopped)
                    return (0, null);
                return Queue.Current;
            }
        }

        public CoreMusicPlayer(DiscordChannel voice, DiscordChannel textChan, IGoogleApiService googleApiService, float volume, CoreMusicService musicService)
        {
            _log = LogManager.GetCurrentClassLogger();
            Volume = volume;
            TextChannel = textChan;
            VoiceChannel = voice; //set voice channel up
            CancellationTokenSource = new CancellationTokenSource();
            _google = googleApiService;
            _musicService = musicService;
            _player = new Thread(new ThreadStart(Player));
            _player.Start();
        }

        private async void Player()
        {
            while (!Exited)
            {
                _bytesSent = 0;
                cancel = false;
                CancellationToken cancellationToken;
                (int Index, MusicInfo song) data;
                lock (locker)
                {
                    data = Queue.Current;
                    cancellationToken = CancellationTokenSource.Token;
                    manualSkip = false;
                    manualIndex = false;
                }

                if (data.song != null)
                {
                    _log.Info($"Starting Player for {VoiceChannel.Name}");
                    CoreMusicHelper seed = null;
                    //try to get voice 
                    try
                    {
                        seed = new CoreMusicHelper(await data.song.Url(), "", data.song.ProviderType == MusicType.Local);
                        _log.Info("Getting voice connection");
                        var ac = await GetVoiceNextConnection();
                        _log.Info("Got voice connection");
                        if (ac == null)
                        {
                            _log.Info("Couldn't join");
                            await TextChannel.SendMessageAsync("Music Player cannot join your voice channel");
                            await Task.Delay(900, cancellationToken);
                            continue;
                        }
                        _log.Info("Created pcm stream");
                        OnStarted?.Invoke(this, data);
                        //using (var ms = new MemoryStream())//hold info if buffer dies on us
                       // {
                            //await seed.outbuf.CopyToAsync(ms).ConfigureAwait(false);
                            var buffer = new byte[3840];
                            var bytesRead = 0;
                            while ((bytesRead = seed.Read(buffer, 0, buffer.Length)) > 0
                                && (MaxPlaytimeSeconds <= 0 || MaxPlaytimeSeconds >= CurrentTime.TotalSeconds))
                            {
                                AdjustVolume(buffer, Volume);
                                if (bytesRead < buffer.Length)
                                {
                                    for (var i = bytesRead; i < buffer.Length; i++)
                                    {
                                        buffer[i] = 0; //just incase the current play time is somehow larger than what we have 
                                    }
                                }
                                await ac.SendAsync(buffer, 20).ConfigureAwait(false);
                                unchecked { _bytesSent += bytesRead; }
                                await (pauseTaskSource?.Task ?? Task.CompletedTask);
                            }
                        //}
                    }
                    catch (OperationCanceledException)
                    {
                        _log.Info("Song Cancelled");
                        cancel = true;
                    }
                    catch (Exception ex)
                    {
                        _log.Warn(ex);
                    }
                    finally
                    {
                        if (seed != null)
                        {
                            seed.Dispose();
                        }
                        OnCompleted?.Invoke(this, data.song);
                        if (_bytesSent == 0 && !cancel)
                        {
                            lock (locker)
                            {
                                Queue.RemoveSong(data.song);
                                _log.Info("Removed song because it cannot be played");
                            }
                        }
                    }
                    try
                    {
                        //repeat song because we can't do anything else
                        int queueCount;
                        bool stopped;
                        int currentIndex;
                        lock (locker)
                        {
                            queueCount = Queue.Count;
                            stopped = Stopped;
                            currentIndex = Queue.CurrIndex;
                        }
                        if (AutoDelete && !RepeatCurrentSong && !RepeatPlaylist && data.song != null)
                        {
                            Queue.RemoveSong(data.song);
                        }

                        if (!manualIndex && (!RepeatCurrentSong || manualSkip))
                        {
                            if (Shuffle)
                            {
                                _log.Info("Random song");
                                Queue.Random(); //if shuffle is set, set current song index to a random number
                            }
                            else
                            {
                                //if last song, and autoplay is enabled, and if it's a youtube song
                                // do autplay magix
                                if (queueCount - 1 == data.Index && Autoplay && data.song?.ProviderType == MusicType.YouTube)
                                {
                                    try
                                    {
                                        _log.Info("Loading related song");
                                        await TextChannel.SendMessageAsync("Loading related song");
                                        await _musicService.TryQueueRelatedSongAsync(data.song, TextChannel, VoiceChannel);
                                        if (!AutoDelete)
                                            Queue.Next();
                                    }
                                    catch
                                    {
                                        _log.Info("Loading related song failed.");
                                        await TextChannel.SendMessageAsync("Loading related song failed.");
                                    }
                                }
                                else if (queueCount - 1 == data.Index && !RepeatPlaylist && !manualSkip)
                                {
                                    _log.Info("Stopping because repeatplaylist is disabled");
                                    await TextChannel.SendMessageAsync("Next Song");
                                    lock (locker)
                                    {
                                        Stop();
                                    }
                                }
                                else
                                {
                                    _log.Info("Next song");
                                    await TextChannel.SendMessageAsync("Next Song");
                                    lock (locker)
                                    {
                                        if (!Stopped)
                                            if (!AutoDelete)
                                                Queue.Next();
                                    }
                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                    do
                    {
                        await Task.Delay(500); //repeate process until Queue is empty or player is stopped
                    }
                    while ((Queue.Count == 0 || Stopped) && !Exited);
                }
            }
        }
        //private async Task<VoiceNextExtension> GetVoiceNextExtensionAsync(bool reconnect = false)
        //{
        //    VoiceChannel.GetConnection(TextChannel.GuildId());
        //}
        private async Task<VoiceNextConnection> GetVoiceNextConnection(bool reconnect = false)
        {
            if (_audioClient == null ||
                _audioClient.IsPlaying ||
                reconnect ||
                newVoiceChannel)
                try
                {
                    try
                    {
                        //check if instance is still available
                        var t =_audioClient?.WaitForPlaybackFinishAsync();
                        if (t != null)
                        {
                            _log.Info("Stopping audio client");
                            await t;
                            _log.Info("Disposing audio client");
                            _audioClient.Dispose();
                        }
                    }
                    catch
                    {
                    }
                    newVoiceChannel = false;
                    //var curUser = await VoiceChannel.Guild.GetChannel
                    //if (curUser?.VoiceState?.Channel != null)
                    //{
                    //    _log.Info("Connecting");
                    //    var ac = await VoiceChannel.ConnectAsync();                        
                    //    _log.Info("Connected, stopping");
                    //    _log.Info("Disconnected");
                    //    ac.Disconnect();
                    //    await Task.Delay(1000);
                    //}
                    _log.Info("Connecting");
                    _audioClient = await VoiceChannel.ConnectAsync();
                }
                catch
                {
                    return null;
                }
            return _audioClient;
        }

        public MusicInfo MoveSong(int n1, int n2)
            => Queue.MoveSong(n1, n2);

        public int Enqueue(MusicInfo song)
        {
            lock (locker)
            {
                if (Exited)
                    return -1;
                Queue.Add(song);
                return Queue.Count - 1;
            }
        }

        public int EnqueueNext(MusicInfo song)
        {
            lock (locker)
            {
                if (Exited)
                    return -1;
                return Queue.AddNext(song);
            }
        }
        public void SetIndex(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            lock (locker)
            {
                if (Exited)
                    return;
                if (AutoDelete && index >= Queue.CurrIndex && index > 0)
                    index--;
                Queue.CurrIndex = index;
                manualIndex = true;
                Stopped = false;
                CancelCurrentSong();
            }
        }
        public void Next(int skipCount = 1)
        {
            lock (locker)
            {
                if (Exited)
                    return;
                manualSkip = true;
                // if player is stopped, and user uses .n, it should play current song.  
                // It's a bit weird, but that's the least annoying solution
                if (!Stopped)
                    if (!RepeatPlaylist && Queue.IsLast()) // if it's the last song in the queue, and repeat playlist is disabled
                    { //stop the queue
                        Stop();
                        return;
                    }
                    else
                        Queue.Next(skipCount - 1);
                else
                    Queue.CurrIndex = 0;
                Stopped = false;
                CancelCurrentSong();
                Unpause();
            }
        }
        //stop playing
        public void Stop(bool clearQueue = false)
        {
            lock (locker)
            {
                Stopped = true;
                if (clearQueue)
                    Queue.Clear();
                Unpause();
                CancelCurrentSong();
            }
        }
        private void Unpause()
        {
            lock (locker)
            {
                if (pauseTaskSource != null)
                {
                    pauseTaskSource.TrySetResult(true);
                    pauseTaskSource = null;
                }
            }
        }
        public void TogglePause()
        {
            lock (locker)
            {
                if (pauseTaskSource == null)
                    pauseTaskSource = new TaskCompletionSource<bool>();
                else
                {
                    Unpause();
                }
            }
            OnPauseChanged?.Invoke(this, pauseTaskSource != null);
        }
        public void SetVolume(float volume)
        {
            if (volume < 0 || volume > 100)
                throw new ArgumentOutOfRangeException(nameof(volume));
            lock (locker)
            {
                Volume = ((float)volume) / 100;
            }
        }
        public MusicInfo RemoveAt(int index)
        {
            lock (locker)
            {
                var cur = Queue.Current;
                var toReturn = Queue.RemoveAt(index);
                if (cur.Index == index)
                    Next();
                return toReturn;
            }
        }
        private void CancelCurrentSong()
        {
            lock (locker)
            {
                var cs = CancellationTokenSource;
                CancellationTokenSource = new CancellationTokenSource();
                cs.Cancel();
            }
        }
        public void ClearQueue()
        {
            lock (locker)
            {
                Queue.Clear();
            }
        }
        public (int CurrentIndex, MusicInfo[] Songs) QueueArray()
        {
            lock (locker)
                return Queue.ToArray();
        }
        ////taken from aidiakapi
        public static unsafe byte[] AdjustVolume(byte[] audioSamples, float volume)
        {
            if (Math.Abs(volume - 1f) < 0.0001f) return audioSamples;

            // 16-bit precision for the multiplication
            var volumeFixed = (int)Math.Round(volume * 65536d);

            var count = audioSamples.Length / 2;

            fixed (byte* srcBytes = audioSamples)
            {
                var src = (short*)srcBytes;

                for (var i = count; i != 0; i--, src++)
                    *src = (short)(((*src) * volumeFixed) >> 16);
            }

            return audioSamples;
        }
        public bool ToggleRepeatSong()
        {
            lock (locker)
            {
                return RepeatCurrentSong = !RepeatCurrentSong;
            }
        }
        public bool ToggleShuffle()
        {
            lock (locker)
            {
                return Shuffle = !Shuffle;
            }
        }

        public bool ToggleAutoplay()
        {
            lock (locker)
            {
                return Autoplay = !Autoplay;
            }
        }
        public bool ToggleRepeatPlaylist()
        {
            lock (locker)
            {
                return RepeatPlaylist = !RepeatPlaylist;
            }
        }

        public async Task SetVoiceChannel(DiscordChannel vch)
        {
            lock (locker)
            {
                if (Exited)
                    return;
                VoiceChannel = vch;
            }
            _audioClient = await vch.ConnectAsync();
        }
        //taken from music module.py from
        public async Task UpdateSongDurationsAsync()
        {
            var sw = Stopwatch.StartNew();
            var (_, songs) = Queue.ToArray();
            var toUpdate = songs
                .Where(x => x.ProviderType == MusicType.YouTube
                    && x.TotalTime == TimeSpan.Zero);

            var vIds = toUpdate.Select(x => x.VideoId);

            sw.Stop();
            _log.Info(sw.Elapsed.TotalSeconds);
            if (!vIds.Any())
                return;

            var durations = await _google.GetVideoDurationsAsync(vIds);

            foreach (var x in toUpdate)
            {
                if (durations.TryGetValue(x.VideoId, out var dur))
                    x.TotalTime = dur;
            }
        }
        public async Task Destroy()
        {
            _log.Info("Destroying Player");
            lock (locker)
            {
                Stop();
                Exited = true;
                Unpause();

                OnCompleted = null;
                OnPauseChanged = null;
                OnStarted = null;
            }
            ///var nvc = await GetVoiceNextConnection();//disconnect
            var nvc = await VoiceChannel.ConnectAsync();
            nvc.Disconnect();
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _audioClient.Disconnect();
        }
    }
}

