using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Core_Discord.CoreServices
{
    /// <summary>
    /// Cretes a process to use YoutubeDL
    /// Donwloads song based on url
    /// </summary>
    public class YoutubeDLOp : IDisposable
    {
        private readonly Logger _log;

        public YoutubeDLOp()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public async Task<string> GetDataAsync(string url)
        {
            using (Process process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "youtube-dl",
                    Arguments = $"-f bestaudio -e --get-url --get-id --get-thumbnail --get-duration --no-check-certificate --default-search \"ytsearch:\" \"{url}\"",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                },
            })
            {
                process.Start();
                var str = await process.StandardOutput.ReadToEndAsync();
                var err = await process.StandardError.ReadToEndAsync();
                if (!string.IsNullOrEmpty(err))
                    _log.Warn(err);
                return str;
            }
        }

        public void Dispose()
        {

        }
    }
}
