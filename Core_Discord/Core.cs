using System;
using DSharpPlus;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using System.IO;
using Core_Discord.CoreMusic;



namespace Core_Discord
{
    public sealed class Core
    {

        private CoreConfig Config { get; set; }
        private DiscordClient Discord { get; set; }
        private CoreCommands Commands { get; }
        private VoiceNextClient VoiceService { get; }
        private CommandsNextModule CommandsNextService { get; }
        private InteractivityModule InteractivityService { get; }
        private Timer TimeGuard { get; set; }


        public Core(CoreConfig config, int shardId)
        {
            this.Config = config;

            var coreConfig = new DiscordConfiguration
            {
                AutoReconnect = true,
                LargeThreshold = 250,
                LogLevel = LogLevel.Debug,
                Token = this.Config.Token,
                TokenType = this.Config.UseUserToken ? TokenType.User : TokenType.Bot,
                UseInternalLogHandler = false,
                ShardId = shardId,
                ShardCount = this.Config.ShardCount,
                EnableCompression = true,
                MessageCacheSize = 50,
                AutomaticGuildSync = !this.Config.UseUserToken,
                DateTimeFormat = "dd-MM-yyyy HH:mm:ss zzz"
            };

            Discord = new DiscordClient(coreConfig);

            //attach Discord events
            Discord.DebugLogger.LogMessageReceived += this.DebugLogger_LogMessageReceived;
            Discord.Ready += this.Discord_Ready;
            Discord.GuildAvailable += this.Discord_GuildAvailable;
            Discord.MessageCreated += this.Discord_MessageCreated;
            Discord.ClientErrored += this.Discord_ClientErrored;
            Discord.SocketErrored += this.Discord_SocketError;
            Discord.GuildCreated += this.Discord_GuildAvailable;

            var voiceConfig = new VoiceNextConfiguration
            {
                VoiceApplication = DSharpPlus.VoiceNext.Codec.VoiceApplication.Music,
                EnableIncoming = false
            };

            //enable voice service
            this.VoiceService = this.Discord.UseVoiceNext(voiceConfig);

            var depoBuild = new DependencyCollectionBuilder();
            depoBuild.Add<CoreMusicPlayer>();

            //add dependency here


            //build command configuration
            //see Dsharpplus configuration
            var commandConfig = new CommandsNextConfiguration
            {
                StringPrefix = this.Config.CommandPrefix,
                EnableDms = true,
                EnableMentionPrefix = true,
                CaseSensitive = true,
                Dependencies = depoBuild.Build(),
                SelfBot = this.Config.UseUserToken,
                IgnoreExtraArguments = false
            };

            //attach command events
            this.CommandsNextService = Discord.UseCommandsNext(commandConfig);
            this.CommandsNextService.CommandErrored += this.CommandsNextService_CommandErrored;
            this.CommandsNextService.CommandExecuted += this.CommandsNextService_CommandExecuted;

            this.CommandsNextService.RegisterCommands(typeof(Core).GetTypeInfo().Assembly);
            this.CommandsNextService.SetHelpFormatter<CoreBotHelpFormatter>();

            //interactive service

            var interConfig = new InteractivityConfiguration()
            {
                PaginationBehaviour = TimeoutBehaviour.Delete,
                //default paginationtimeout (30 seconds)
                PaginationTimeout = TimeSpan.FromSeconds(30),
                //timeout for current action
                Timeout = TimeSpan.FromMinutes(2) 
            };

            //attach interactive component
            this.InteractivityService = Discord.UseInteractivity(interConfig);
           //this.CommandsNextService.RegisterCommands<CoreInteractivityModuleCommands>();
            //register commands from coreinteractivitymodulecommands
            //this.CommandsNextService.RegisterCommands(typeof(CoreInteractivityModuleCommands).GetTypeInfo().Assembly); 

        }

        public async Task RunAsync()
        {
            await Discord.ConnectAsync().ConfigureAwait(false);
            await Task.Delay(-1).ConfigureAwait(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[{0:yyyy-MM-dd HH:mm:ss zzz}] ", e.Timestamp.ToLocalTime());

            var tag = e.Application;
            if (tag.Length > 12)
                tag = tag.Substring(0, 12);
            if (tag.Length < 12)
                tag = tag.PadLeft(12, ' ');
            Console.Write("[{0}] ", tag);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[{0}] ", string.Concat("SHARD ", this.Discord.ShardId.ToString("00")));

            switch (e.Level)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
            }
            Console.Write("[{0}] ", e.Level.ToString().PadLeft(8));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(e.Message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"> Sets Ready Events</param>
        /// <returns>
        /// Task Complete
        /// </returns>
        private Task Discord_Ready(ReadyEventArgs e)
        {
            if (!this.Config.UseUserToken)
                this.TimeGuard = new Timer(TimerCallback, null, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(15));
            return Task.Delay(0);
        }
        /// <summary>
        /// Provides updates on console
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Task Discord_GuildAvailable(GuildCreateEventArgs e)
        {
            Discord.DebugLogger.LogMessage(LogLevel.Info, "DSPlus Test", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.Delay(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Task Discord_GuildCreated(GuildCreateEventArgs e)
        {
            Discord.DebugLogger.LogMessage(LogLevel.Info, "DSPlus Test", $"Guild created: {e.Guild.Name}", DateTime.Now);
            return Task.Delay(0);
        }
        /// <summary>
        /// Default message when bot is mentioned but no command specified
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task Discord_MessageCreated(MessageCreateEventArgs e)
        {
            if (e.Message.Content.Contains($"<@!{e.Client.CurrentUser.Id}>") || e.Message.Content.Contains($"<@{e.Client.CurrentUser.Id}>"))
                await e.Message.RespondAsync("r u havin' a ggl thr m8").ConfigureAwait(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Task Discord_ClientErrored(ClientErrorEventArgs e)
        {
            this.Discord.DebugLogger.LogMessage(LogLevel.Error, "DSP Test", $"Client threw an exception: {e.Exception.GetType()}", DateTime.Now);
            return Task.Delay(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Task Discord_SocketError(SocketErrorEventArgs e)
        {
            this.Discord.DebugLogger.LogMessage(LogLevel.Error, "WebSocket", $"WS threw an exception: {e.Exception.GetType()}", DateTime.Now);
            return Task.Delay(0);
        }
        /// <summary>
        /// Checks when a command returns an exception during execution of Task Command, if it failed due to error from the method
        /// exception, builds an error report (generally this is for developers)
        /// </summary>
        /// <param name="e"></param>
        /// <returns> Message with a exception text file built  </returns>
        private async Task CommandsNextService_CommandErrored(CommandErrorEventArgs e)
        {
            if (e.Exception is CommandNotFoundException && (e.Command == null || e.Command.QualifiedName != "help"))
                return;

            Discord.DebugLogger.LogMessage(LogLevel.Error, "CommandsNext", $"An exception occured during {e.Context.User.Username}'s invocation of '{e.Context.Command.QualifiedName}': {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            if(e.Exception is UnauthorizedAccessException)
            {
                //check if user lacks permissions
                //and provides notice

                var errEmoji = DiscordEmoji.FromName(e.Context.Client,":no_entry:");

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access Denied",
                    Description = $"{errEmoji} You do not have permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) //red
                };
                await e.Context.RespondAsync("", embed: embed).ConfigureAwait(false);
            }

            var exs = new List<Exception>();
            if (e.Exception is AggregateException ae)
                exs.AddRange(ae.InnerExceptions);
            else
                exs.Add(e.Exception);

            foreach (var ex in exs)
            {
                if (ex is CommandNotFoundException && (e.Command == null || e.Command.QualifiedName != "help"))
                    return;

                var ms = ex.Message;
                var st = ex.StackTrace;

                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write($"{e.Exception.GetType()} occured when executing {e.Command.QualifiedName}.\n\n{ms}\n{st}");
                writer.Flush();
                stream.Position = 0;

                //build the message to user
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#FF0000"),
                    Title = "An exception occured when executing a command",
                    Description = $"`{e.Exception.GetType()}` occured when executing `{e.Command.QualifiedName}`.",
                    Timestamp = DateTime.UtcNow
                };
                embed.WithFooter(Discord.CurrentUser.Username, Discord.CurrentUser.AvatarUrl)
                    .AddField("Message", "File with full details has been attached.", false);
                await e.Context.Channel.SendFileAsync(stream, "error.txt", "\u200b", embed: embed.Build()).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// Logs execution of command from user and from which channel in the Discord server
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Task CommandsNextService_CommandExecuted(CommandExecutionEventArgs e)
        {
            Discord.DebugLogger.LogMessage(LogLevel.Info, "CommandsNext", $"{e.Context.User.Username} executed '{e.Command.QualifiedName}' in {e.Context.Channel.Name}", DateTime.Now);
            return Task.Delay(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_"></param>
        private void TimerCallback(object _)
        {
            try
            {
                this.Discord.UpdateStatusAsync(new DiscordGame("CS 476 Project")).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception) { }
        }
    }
}
