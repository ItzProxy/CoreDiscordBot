using System;
using System.Threading.Tasks;
using Core_Discord.CoreExtensions;
using System.Text.RegularExpressions;
using DSharpPlus;

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
        public string Query { get; set; }
        public string Title { get; set; } //title of song
        public Func<Task<string>> Uri { get; set; } //url of file (file.path or http)
        public string Thumbnail { get; set; } //variable for the thumbnail of video/file
        public string QuerierName { get; set; }
        public TimeSpan TotalTime { get; set; } = TimeSpan.Zero; // sets time

        //format above information for display
        public string FormattedProvider => (Provider ?? "???");
        public string FormattedName => $"[{Title.TrimTo(65)}]({SongUrl})]";
        public string FormattedInfo => $"{FormattedTotalTime} | {FormattedProvider} | {QuerierName}";
        public string FormattedFullName => $"{FormattedName}\n\t\t`{FormattedTotalTime} | {FormattedProvider} | {Formatter.Sanitize(QuerierName.TrimTo(15))}`";
        public string FormattedTotalTime
        {
            get
            {
                if (TotalTime == TimeSpan.Zero)
                    return "(?)";
                if (TotalTime == TimeSpan.MaxValue)
                    return "∞";
                var time = TotalTime.ToString(@"mm\:ss");
                var hrs = (int)TotalTime.TotalHours;

                if (hrs > 0)
                    return hrs + ":" + time;
                return time;
            }
        }
        /// <summary>
        /// determines the url source and gets the song url
        /// </summary>
        public string SongUrl
        {
            get
            {
                switch (ProviderType)
                {
                    case MusicType.Youtube:
                        return Query;
                    case MusicType.Soundcloud:
                        return Query;
                    case MusicType.Local:
                        return "https://google.com/search?q={ WebUtility.UrlEncode(Title).Replace(' ', '+') }";
                    default:
                        return "";
                }
            }
        }

        public string _videoId = null;
        /// <summary>
        /// Video Id: see if matches the query url
        /// </summary>
        public string VideoId
        {
            get
            {
                if (ProviderType == MusicType.Youtube)
                    return _videoId ?? videoIdRegex.Match(Query)?.ToString();
                return _videoId ?? "";
            }
        }

        private readonly Regex videoIdRegex = new Regex("<=v=[a-zA-Z0-9-]+(?=&)|(?<=[0-9])[^&\n]+|(?<=v=)[^&\n]+", RegexOptions.Compiled);
    }
}
