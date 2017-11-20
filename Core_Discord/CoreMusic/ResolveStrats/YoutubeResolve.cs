using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Core_Discord.CoreServices;
using Core_Discord.CoreDatabase.Models;
using System.Globalization;

namespace Core_Discord.CoreMusic.ResolveStrats
{
    public class YoutubeResolve : IResolver
    {
        private readonly Logger _log;

        public YoutubeResolve()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public async Task<MusicInfo> ResolveSong(string query)
        {
            _log.Info("Getting link");
            string[] data;
            try
            {
                using (var youdl = new YoutubeDLOp())
                {
                    data = (await youdl.GetDataAsync(query)).Split('\n');
                }
                if (data.Length < 6)
                {
                    _log.Info("No song found. Data less than 6");
                    return null;
                }
                TimeSpan time;
                if (!TimeSpan.TryParseExact(data[4], new[] { "ss", "m\\:ss", "mm\\:ss", "h\\:mm\\:ss", "hh\\:mm\\:ss", "hhh\\:mm\\:ss" }, CultureInfo.InvariantCulture, out time))
                    time = TimeSpan.FromHours(24);

                return new MusicInfo()
                {
                    Title = data[0],
                    _videoId = data[1],
                    Url = async () =>
                    {
                        using (var youtubeDl = new YoutubeDLOp())
                        {
                            data = (await youtubeDl.GetDataAsync(query)).Split('\n');
                        }
                        if (data.Length < 6)
                        {
                            _log.Info("No song found. Data less than 6");
                            return null;
                        }
                        return data[2];
                    },
                    Thumbnail = data[3],
                    TotalTime = time,
                    Provider = "YouTube",
                    ProviderType = MusicType.YouTube,
                    Query = "https://youtube.com/watch?v=" + data[1],
                };
            }
            catch (Exception ex)
            {
                _log.Warn(ex);
                return null;
            }
        }
    }
}
