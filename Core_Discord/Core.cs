using System;
using DSharpPlus;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using System.IO;
using Core_Discord.CoreDatabase.Models;
using Core_Discord.CoreServices;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System.Diagnostics;
using Core_Discord.CoreMusic;
using Core_Discord.CoreServices.Interfaces;
using System.Linq;
using Core_Discord.CoreDatabase;

namespace Core_Discord
{
    public sealed class Core
    {
        private Logger _log;

        public CoreServiceProvider Services { get; private set; }
        public CoreCredentials Credentials { get; }
        private readonly BotConfig _config;
        public DiscordClient _discord { get; set; }
        private CoreCommands Commands { get; }
        private VoiceNextExtension VoiceService { get; set; }
        private CommandsNextExtension CommandsNextService { get; }
        private InteractivityExtension InteractivityService { get; }
        private Timer TimeGuard { get; set; }
        private readonly DbService _db;

        public Core(int ParentId, int shardId)
        {
            //check if shardId assigned is < 0
            if(shardId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shardId));
            }
            
            //set up credentials
            LogSetup.LoggerSetup(shardId);
            _config = new BotConfig();
            _log = LogManager.GetCurrentClassLogger();
            Credentials = new CoreCredentials();
            _db = new DbService(Credentials);
            var coreConfig = new DiscordConfiguration
            {
                AutoReconnect = true,
                LargeThreshold = 250,
                LogLevel = DSharpPlus.LogLevel.Debug,
                Token = Credentials.Token,
                TokenType = Credentials.UseUserToken ? TokenType.Bot : TokenType.Bot,
                UseInternalLogHandler = false,
                ShardId = shardId,
                ShardCount = Credentials.TotalShards,
                GatewayCompressionLevel = GatewayCompressionLevel.Stream,
                MessageCacheSize = 50,
                AutomaticGuildSync = true,
                DateTimeFormat = "dd-MM-yyyy HH:mm:ss zzz"
            };

            _discord = new DiscordClient(coreConfig);

            //attach Discord events
            _discord.DebugLogger.LogMessageReceived += this.DebugLogger_LogMessageReceived;
            _discord.Ready += this.Discord_Ready;
            _discord.GuildAvailable += this.Discord_GuildAvailable;
            _discord.MessageCreated += this.Discord_MessageCreated;
            _discord.ClientErrored += this.Discord_ClientErrored;
            _discord.SocketErrored += this.Discord_SocketError;
            _discord.GuildCreated += this.Discord_GuildAvailable;
            _discord.VoiceStateUpdated += this.Discord_VoiceStateUpdated;

            var voiceConfig = new VoiceNextConfiguration
            {
                VoiceApplication = DSharpPlus.VoiceNext.Codec.VoiceApplication.Music,
                EnableIncoming = false
            };
            _discord.UseVoiceNext(voiceConfig);
            var googleService = new GoogleApiService(Credentials);
            //enable voice service

            IGoogleApiService googleApiService = new GoogleApiService(Credentials);
            CoreMusicService cms = new CoreMusicService(_discord, _db, Credentials, this,googleApiService);
            var depoBuild = new ServiceCollection();
            //taken from NadeoBot's Service loading
            depoBuild.AddSingleton<DiscordClient>(_discord);
            depoBuild.AddSingleton<CoreCredentials>(Credentials);
            depoBuild.AddSingleton<IGoogleApiService>(googleApiService);
            depoBuild.AddSingleton(_db);
            depoBuild.AddSingleton(cms);

            //add dependency here

            using (var uow = _db.UnitOfWork)
            {
                _config = uow.BotConfig.GetOrCreate();
            }

            //build command configuration
            //see Dsharpplus configuration
            _log.Info($"{_config.DefaultPrefix}");

            var commandConfig = new CommandsNextConfiguration
            {
                StringPrefix = _config.DefaultPrefix,
                EnableDms = true,
                EnableMentionPrefix = true,
                CaseSensitive = true,
                Services = depoBuild.BuildServiceProvider(),
                Selfbot = Credentials.UseUserToken,
                IgnoreExtraArguments = false
            };

            //attach command events
            this.CommandsNextService = _discord.UseCommandsNext(commandConfig);

            this.CommandsNextService.CommandErrored += this.CommandsNextService_CommandErrored;

            this.CommandsNextService.CommandExecuted += this.CommandsNextService_CommandExecuted;

            this.CommandsNextService.RegisterCommands(typeof(CoreCommands).GetTypeInfo().Assembly);

            this.CommandsNextService.SetHelpFormatter<CoreBotHelpFormatter>();

            //interactive service

            var interConfig = new InteractivityConfiguration()
            {
                PaginationBehaviour = TimeoutBehaviour.DeleteMessage,
                //default paginationtimeout (30 seconds)
                PaginationTimeout = TimeSpan.FromSeconds(30),
                //timeout for current action
                Timeout = TimeSpan.FromMinutes(2) 
            };

            //attach interactive component
            this.InteractivityService = _discord.UseInteractivity(interConfig);
           //this.CommandsNextService.RegisterCommands<CoreInteractivityModuleCommands>();
            //register commands from coreinteractivitymodulecommands
            //this.CommandsNextService.RegisterCommands(typeof(CoreInteractivityModuleCommands).GetTypeInfo().Assembly); 

        }
        #region StartBot
        /*
        private void SetupShard(int parentProcessId)
        {
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    var p = Process.GetProcessById(parentProcessId);
                    if (p == null)
                        return;
                    p.WaitForExit();
                }
                finally
                {
                    Environment.Exit(10);
                }
            })).Start();
        }
        public async Task RunAndBlockAsync(params string[] args)
        {
            await RunAsync(args).ConfigureAwait(false);
            await Task.Delay(-1).ConfigureAwait(false);
        }
        public async Task RunAsync(params string[] args)
        {
            var sw = Stopwatch.StartNew();

            await _discord.ConnectAsync();

            _log.Info($"Shard {_discord.ShardId} loading services...");
            try
            {
                AddServices();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }

            sw.Stop();
            _log.Info($"Shard {_discord.ShardId} connected in {sw.Elapsed.TotalSeconds:F2}s");

            var stats = Services.GetService<IStatsService>();
            stats.Initialize();
            var commandHandler = Services.GetService<CommandHandler>();
            var CommandService = Services.GetService<CommandService>();

            // start handling messages received in commandhandler
            await commandHandler.StartHandling().ConfigureAwait(false);

            var _ = await CommandService.AddModulesAsync(this.GetType().GetTypeInfo().Assembly);

            //unload modules which are not available on the public bot

            Ready.TrySetResult(true);
            HandleStatusChanges();
            StartSendingData();
            _log.Info($"Shard {Client.ShardId} ready.");
        }
        */
        #endregion
        public async Task RunAsync()
        {
            await _discord.ConnectAsync().ConfigureAwait(false);
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
            Console.Write("[{0}] ", string.Concat("SHARD ", this._discord.ShardId.ToString("00")));

            switch (e.Level)
            {
                case DSharpPlus.LogLevel.Critical:
                case DSharpPlus.LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case DSharpPlus.LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case DSharpPlus.LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                case DSharpPlus.LogLevel.Debug:
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
            if (!this.Credentials.UseUserToken)
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
            _discord.DebugLogger.LogMessage(DSharpPlus.LogLevel.Info, "DSPlus Test", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.Delay(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private Task Discord_GuildCreated(GuildCreateEventArgs e)
        {
            _discord.DebugLogger.LogMessage(DSharpPlus.LogLevel.Info, "DSPlus Test", $"Guild created: {e.Guild.Name}", DateTime.Now);
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
            this._discord.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "DSP Test", $"Client threw an exception: {e.Exception.GetType()}", DateTime.Now);
            return Task.Delay(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Task</returns>
        private Task Discord_SocketError(SocketErrorEventArgs e)
        {
            this._discord.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "WebSocket", $"WS threw an exception: {e.Exception.GetType()}", DateTime.Now);
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
            {
                await e.Context.RespondAsync($"Command does not exist...refer to '{e.Context.Prefix}help'");
                return;
            }


            _discord.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error, "CommandsNext", $"An exception occured during {e.Context.User.Username}'s invocation of '{e.Context.Command.QualifiedName}': {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

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
                embed.WithFooter(_discord.CurrentUser.Username, _discord.CurrentUser.AvatarUrl)
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
            _discord.DebugLogger.LogMessage(DSharpPlus.LogLevel.Info, "CommandsNext", $"{e.Context.User.Username} executed '{e.Command.QualifiedName}' in {e.Context.Channel.Name}", DateTime.Now);
            return Task.Delay(0);
        }

        private Task Discord_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
        {
            this._discord.DebugLogger.LogMessage(DSharpPlus.LogLevel.Debug, "DSP Test", $"Voice state change for {e.User}: {e.Before?.IsServerMuted}->{e.After.IsServerMuted} {e.Before?.IsServerDeafened}->{e.After.IsServerDeafened}", DateTime.Now);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_"></param>
        private void TimerCallback(object _)
        {
            try
            {
                this._discord.UpdateStatusAsync(new DiscordActivity("CS 476 Project")).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception) { }
        }
        /*
        //running out of time
        private void AddServices()
        {
            var startingGuildIdList = _discord.Guilds.Select(x => (long)x.Value.Id).ToList();

            //this unit of work will be used for initialization of all modules too, to prevent multiple queries from running
            using (var uow = _db.UnitOfWork)
            {

                IBotConfigProvider botConfigProvider = new BotConfigProvider(_db, _config);

                //initialize Services
                Services = new CoreServiceProvider()
                    .AddManual<CoreCredentials>(Credentials)
                    .AddManual(_db)
                    .AddManual(_discord)
                    .AddManual(botConfigProvider)
                    .AddManual<Core>(this)
                    .AddManual<IUnitOfWork>(uow);

                Services.LoadFrom(Assembly.GetAssembly(typeof(CommandHandler)));

                var commandHandler = Services.GetService<CommandHandler>();
                commandHandler.AddServices(Services);

                LoadTypeReaders(typeof(NadekoBot).Assembly);
            }
            Services.Unload(typeof(IUnitOfWork)); // unload it after the startup
        }

        private IEnumerable<object> LoadTypeReaders(Assembly assembly)
        {
            Type[] allTypes;
            try
            {
                allTypes = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine(ex.LoaderExceptions[0]);
                return Enumerable.Empty<object>();
            }
            var filteredTypes = allTypes
                .Where(x => x.IsSubclassOf(typeof(TypeReader))
                    && x.BaseType.GetGenericArguments().Length > 0
                    && !x.IsAbstract);

            var toReturn = new List<object>();
            foreach (var ft in filteredTypes)
            {
                var x = (TypeReader)Activator.CreateInstance(ft, Client, CommandService);
                var baseType = ft.BaseType;
                var typeArgs = baseType.GetGenericArguments();
                try
                {
                    CommandService.AddTypeReader(typeArgs[0], x);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    throw;
                }
                toReturn.Add(x);
            }

            return toReturn;
        }
        */
    }
}
