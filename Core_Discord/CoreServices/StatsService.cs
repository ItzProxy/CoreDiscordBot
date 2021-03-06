﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core_Discord.CoreServices.Interfaces;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Http;
using System.Security.Cryptography;
using System.Globalization;
using Core_Discord.CoreServices;
namespace Core_Discord.CoreServices
{
    public class StatsService : IStatNextService
    {
        private readonly DiscordClient _client;
        private readonly ICoreCredentials _creds;
        private readonly DateTime _started;

        public const string BotVersion = "2.4.4";

        public string Author => "ItzProxy#0638";
        public string Library => "DSharplus";
        public string Heap => Math.Round((double)GC.GetTotalMemory(false) / 1.MiB(), 2)
            .ToString(CultureInfo.InvariantCulture);
        public double MessagesPerSecond => MessageCounter / GetUptime().TotalSeconds;

        private long _textChannels;
        public long TextChannels => Interlocked.Read(ref _textChannels);
        private long _voiceChannels;
        public long VoiceChannels => Interlocked.Read(ref _voiceChannels);
        private long _messageCounter;
        public long MessageCounter => Interlocked.Read(ref _messageCounter);
        private long _commandsRan;
        public long CommandsRan => Interlocked.Read(ref _commandsRan);

        public double MessagePerSecond => throw new NotImplementedException();

        public long Channels => throw new NotImplementedException();

        private readonly Timer _carbonitexTimer;
        private readonly Timer _dataTimer;
        private readonly ConnectionMultiplexer _redis;

        public StatsService(DiscordClient client, CommandHandler cmdHandler,
            Core creds, Core nadeko,
            IDataCache cache)
        {
            _client = client;
            _creds = creds;
            _redis = cache.Redis;

            _started = DateTime.UtcNow;
            _client.MessageReceived += _ => Task.FromResult(Interlocked.Increment(ref _messageCounter));
            cmdHandler.CommandExecuted += (_, e) => Task.FromResult(Interlocked.Increment(ref _commandsRan));

            _client.ChannelCreated += (c) =>
            {
                var _ = Task.Run(() =>
                {
                    if (c is ITextChannel)
                        Interlocked.Increment(ref _textChannels);
                    else if (c is IVoiceChannel)
                        Interlocked.Increment(ref _voiceChannels);
                });

                return Task.CompletedTask;
            };

            _client.ChannelDestroyed += (c) =>
            {
                var _ = Task.Run(() =>
                {
                    if (c is ITextChannel)
                        Interlocked.Decrement(ref _textChannels);
                    else if (c is IVoiceChannel)
                        Interlocked.Decrement(ref _voiceChannels);
                });

                return Task.CompletedTask;
            };

            _client.GuildAvailable += (g) =>
            {
                var _ = Task.Run(() =>
                {
                    var tc = g.Channels.Count(cx => cx is ITextChannel);
                    var vc = g.Channels.Count - tc;
                    Interlocked.Add(ref _textChannels, tc);
                    Interlocked.Add(ref _voiceChannels, vc);
                });
                return Task.CompletedTask;
            };

            _client.JoinedGuild += (g) =>
            {
                var _ = Task.Run(() =>
                {
                    var tc = g.Channels.Count(cx => cx is ITextChannel);
                    var vc = g.Channels.Count - tc;
                    Interlocked.Add(ref _textChannels, tc);
                    Interlocked.Add(ref _voiceChannels, vc);
                });
                return Task.CompletedTask;
            };

            _client.GuildUnavailable += (g) =>
            {
                var _ = Task.Run(() =>
                {
                    var tc = g.Channels.Count(cx => cx is ITextChannel);
                    var vc = g.Channels.Count - tc;
                    Interlocked.Add(ref _textChannels, -tc);
                    Interlocked.Add(ref _voiceChannels, -vc);
                });

                return Task.CompletedTask;
            };

            _client.GuildDeleted += (g) =>
            {
                var _ = Task.Run(() =>
                {
                    var tc = g.Channels.Count(cx => cx is ITextChannel);
                    var vc = g.Channels.Count - tc;
                    Interlocked.Add(ref _textChannels, -tc);
                    Interlocked.Add(ref _voiceChannels, -vc);
                });

                return Task.CompletedTask;
            };

            if (_client.ShardId == 0)
            {
                _carbonitexTimer = new Timer(async (state) =>
                {
                    if (string.IsNullOrWhiteSpace(_creds.CarbonKey))
                        return;
                    try
                    {
                        using (var http = new HttpClient())
                        {
                            using (var content = new FormUrlEncodedContent(
                                new Dictionary<string, string> {
                                { "servercount", nadeko.GuildCount.ToString() },
                                { "key", _creds.CarbonKey }}))
                            {
                                content.Headers.Clear();
                                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                                await http.PostAsync("https://www.carbonitex.net/discord/data/botdata.php", content).ConfigureAwait(false);
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }, null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));

                var platform = "other";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    platform = "linux";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    platform = "osx";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    platform = "windows";

                _dataTimer = new Timer(async (state) =>
                {
                    try
                    {
                        using (var http = new HttpClient())
                        {
                            using (var content = new FormUrlEncodedContent(
                                new Dictionary<string, string> {
                                    { "id", string.Concat(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(_creds.ClientId.ToString())).Select(x => x.ToString("X2"))) },
                                    { "guildCount", nadeko.GuildCount.ToString() },
                                    { "version", BotVersion },
                                    { "platform", platform }}))
                            {
                                content.Headers.Clear();
                                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                                await http.PostAsync("https://selfstats.nadekobot.me/", content).ConfigureAwait(false);
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }, null, TimeSpan.FromSeconds(1), TimeSpan.FromHours(1));
            }
        }

        public void Initialize()
        {
            var guilds = _client.Guilds.ToArray();
            _textChannels = guilds.Sum(g => g.Channels.Count(cx => cx is ITextChannel));
            _voiceChannels = guilds.Sum(g => g.Channels.Count) - _textChannels;
        }

        public Task<string> Print()
        {
            DiscordUser curUser;
            while ((curUser = _client.CurrentUser) == null) Task.Delay(1000).ConfigureAwait(false);

            return Task.FromResult($@"
Author: [{Author}] | Library: [{Library}]
Bot Version: [{BotVersion}]
Bot ID: {curUser.Id}
Owner ID(s): {string.Join(", ", _creds.OwnerIds)}
Uptime: {GetUptimeString()}
Servers: {_client.Guilds.Count} | TextChannels: {TextChannels} | VoiceChannels: {VoiceChannels}
Commands Ran this session: {CommandsRan}
Messages: {MessageCounter} [{MessagesPerSecond:F2}/sec] Heap: [{Heap} MB]");
        }

        public TimeSpan GetUptime() =>
            DateTime.UtcNow - _started;

        public string GetUptimeString(string separator = ", ")
        {
            var time = GetUptime();
            return $"{time.Days} days{separator}{time.Hours} hours{separator}{time.Minutes} minutes";
        }
    }
}
