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

        private TaskCompletionSource<bool> pauseTaskSource { get; set; } = null;
        public float Volume { get; private set; } = 1.0f;
        public bool Stopped { get; private set; } = false;
        public 

        public string VolumeIcon => $"🔉 {(int)(Volume * 100)}%";

        public string CurrentTime
        {
            get
            {
                var time = CurrentTime.ToString(@"mm\:ss");
                var hrs = (int)CurrentTime.TotalHours;
            }
        }

    }
}
