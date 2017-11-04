using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreDatabase.Models
{
    public class PlaylistUser
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public ulong AuthorId { get; set; }
        public List<PlaylistSong> Songs { get; set; } = new List<PlaylistSong>();
    }
}
