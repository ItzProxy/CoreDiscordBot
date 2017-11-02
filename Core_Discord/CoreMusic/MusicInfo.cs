using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core_Discord.CoreMusic
{
    /// <summary>
    /// What type of provider the song came from
    /// </summary>
    public enum MusicType
    {
        Youtube,
        Local,
        Soundcloud

    }
    public class MusicInfo
    {
        
        public string Provider { get; set; } //Where file came from 
        public MusicType ProviderType { get; set; } //What type provider provided the file
        public string Title { get; set; } //title of song
        public Func<Task<string>> Uri { get; set; } //url of file (file.path or http)
        public string Thumbnail { get; set; } //variable for the thumbnail of video/file
        public string QueryName { get; set; }
        public TimeSpan TotalTime { get; set; } = TimeSpan.Zero; // sets time

        //format above information for display
        public string FormattedProvider => (Provider ?? "???");
        public string FormattedInfo => $"*"[{Title.TrimTo]

    }
}
