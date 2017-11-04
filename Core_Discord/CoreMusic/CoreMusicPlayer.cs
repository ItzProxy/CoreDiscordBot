using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.VoiceNext;
using DSharpPlus.Exceptions;

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
    public sealed class CoreMusicPlayer
    {

        private readonly Thread _player;
        public VoiceNextClient VoiceChannel {get; private set;}
        private readonly Logger

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

                if(hrs > 0)
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

        public CoreMusicPlayer(VoiceNextClient voice)
        {
            VoiceChannel = voice; //set voice channel up
        }
    }
}
