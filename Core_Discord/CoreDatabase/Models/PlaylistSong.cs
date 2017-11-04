using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreDatabase.Models
{
    public class PlaylistSong : DbEntity
    {
        public string Provider { get; set; }
        public MusicType ProviderType { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
        public string Query { get; set; }
    }

    public enum MusicType
    {
        YouTube,
        Local,
        Soundcloud
    }
}
