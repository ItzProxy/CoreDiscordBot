using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using System.Threading;
using System.Diagnostics;
using System.IO;
/// <summary>
/// Built using an Outdated Music Player for Discord
/// Retooled to help play music through voice channel
/// </summary>

namespace Core_Discord.Music
{
    public class CoreMusicHelper : IDisposable
    {
        const int size = 65536; // 2^16
        private Process pro;
        private Stream outbuf;

        private SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly Logger _log;

        public string SongUrl { get; private set; }

        private readonly bool _isLocal; //check if file is locally found in computer
        private readonly object locker = new object();

        public CoreMusicHelper(string songUrl, string skipTo, bool isLocal)
        {
            _log = LogManager.GetCurrentClassLogger(); //initilize logger class
            SongUrl = songUrl;
            _isLocal = isLocal;

            try
            {
                this.pro = StartFFmegProcess(SongUrl, 0);
                outbuf = this.pro.StandardOutput.BaseStream;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                _log.Error($"FFMPEG not installed or configured properly");
            }
            catch (OperationCanceledException) { }
            catch (InvalidOperationException) { } //ffmpeg disposed
            catch(Exception ex)
            {
                _log.Info(ex);
            }

        }

        private Process StartFFmegProcess(string SongUrl, float skipTo = 0)
        {

            var args = $"-err_detect ignore_err -i {SongUrl} -f s16le -ar 48000 -vn -ac 2 pipe:1 -loglevel error";
            if (!_isLocal)
                args = "-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 " + args;
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                CreateNoWindow = true
            });
        }

        public int Read(byte[] b, int offset, int toRead)
        {
            lock (locker)
            {
                return outbuf.Read(b, offset, toRead);
            }
        }

        public void Dispose()
        {
            try
            {
                this.pro.StandardOutput.Dispose();
            }
            catch(Exception e)
            {
                _log.Error(e);
            }
            try
            {
                if (!pro.HasExited) //if process not killed, kill it
                {
                    pro.Kill();
                }
            }
            catch { }
            outbuf.Dispose();
            pro.Dispose();
        }
    }
}
