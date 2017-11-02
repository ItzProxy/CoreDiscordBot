using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreMusic
{
    public class CoreMusicExceptions
    {
        public class NotInVOiceChannelException : Exception
        {
            public NotInVOiceChannelException() : base("You're not in a voice channel on this server.") { }
        }

        public class QueueFullException : Exception
        {
            public QueueFullException(string message) : base(message) { }
            public QueueFullException() : base("Queue is full.") { }
        }

        public class SongNotFoundException : Exception
        {
            public SongNotFoundException(string message) : base(message) { }
            public SongNotFoundException() : base("Song is not found") { }
        }

        public class SongNull : Exception
        {
            public SongNull(string message) : base(message) { }
            public SongNull() : base("Song is invalid") { }
        }
    }
}
