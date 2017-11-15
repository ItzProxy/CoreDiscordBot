using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreDatabase.Models
{
    /// <summary>
    /// Holds the structure for play list
    /// <name> Name of play list</name>
    /// <author>Name of the Discord User who made this playlist</author>
    /// <authorId>the discord users numerical tag</authorId>
    /// <Songs> The songs in playlist that is instance as an empty list</Songs>
    /// </summary>
    public class PlaylistUser : DbEntity
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public long AuthorId { get; set; }
        public List<PlaylistSong> Songs { get; set; } = new List<PlaylistSong>();
    }
}
