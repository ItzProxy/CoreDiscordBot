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
    public partial class CoreMusicPlayer
    {
        private Logger _log;
        private readonly object locker = new object(); //semaphore
        private readonly Thread _player;
        public VoiceNextExtension VoiceChannel { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public DiscordChannel TextChannel { get; set; }
        private readonly IGoogleApiService _google;
        private CoreMusicService _musicService;
        //logger

        private CoreMusicQueue Queue { get; } = new CoreMusicQueue();

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

        public CoreMusicPlayer(VoiceNextExtension voice, DiscordChannel textChan, IGoogleApiService googleApiService, float volume, CoreMusicService musicService)
        {
            _log = LogManager.GetCurrentClassLogger();
            Volume = volume;
            TextChannel = textChan;
            VoiceChannel = voice; //set voice channel up
            CancellationTokenSource = new CancellationTokenSource();
            _google = googleApiService;
            _musicService = musicService;
            _player = new Thread(new ThreadStart(Player));
        }

        private async void Player()
        {
            _bytesSent = 0;
            cancel = false;
            CancellationToken cancellationToken;
            (int Index, MusicInfo song) data;
            lock (locker) {
                data = Queue.Current;
                cancellationToken = CancellationTokenSource.Token;
                manualSkip = false;
                manualIndex = false;
            }

            if (data.song != null)
            {
                _log.Info($"Starting Player for {TextChannel.Name}");
                CoreMusicHelper buffer = null;
                //try to get voice 
                try
                {
                    buffer = new CoreMusicHelper(await data.song.Url(), "", data.song.ProviderType == MusicType.Local);
                    //var ac = await
                    //if (ac == null)
                    //{
                    //    VoiceChannel.
                    //}
                }
                catch
                {

                }
            }

        }

        public MusicInfo MoveSong(int n1, int n2)
            => Queue.MoveSong(n1, n2);
        private async void PlayerLoop()
        {
            throw new NotImplementedException();
        }

        public int Enqueue(MusicInfo song)
        {
            throw new NotImplementedException();
        }
        public int EnqueueNext(MusicInfo song)
        {
            throw new NotImplementedException();
        }
        public void SetIndex(int index)
        {
            throw new NotImplementedException();
        }
        public void Next(int skipCount = 1)
        {
            throw new NotImplementedException();
        }
        public void Stop(bool clearQueue = false)
        {
            throw new NotImplementedException();
        }
        private void Unpause()
        {
            throw new NotImplementedException();
        }
        public void TogglePause()
        {
            throw new NotImplementedException();
        }
        public void SetVolume(float volume){
            throw new NotImplementedException();
        }
        public MusicInfo RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
        private void CancelCurrentSong()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        public bool ToggleShuffle()
        {
            throw new NotImplementedException();
        }

        public bool ToggleAutoplay()
        {
            throw new NotImplementedException();
        }
        public bool ToggleRepeatPlaylist()
        {
            throw new NotImplementedException();
        }

        public async Task SetVoiceChannel(VoiceNextExtension vch)
        {
            lock (locker)
            {
                if (Exited)
                    return;
                //VoiceChannel = vch;
            }
           // _audioClient = await vch.ConnectAsync(VoiceChannel);
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
            await VoiceChannel.Client.DisconnectAsync(); //disconnect
            await Task.CompletedTask;
        }
    }
}
