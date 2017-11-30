using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Core_Discord
{
    internal sealed class Program
    {
        //public static void Main(string[] arg)
        //{
            //trying to truely multi thread, but decided to just make it so the OS does the threading
            //            if (arg.Length == 2
            //                && int.TryParse(arg[0], out int shardId)
            //                && int.TryParse(arg[1], out int parentProcessId))
            //            {
            //                return new Core(shardId, parentProcessId)
            //                    .RunAndBlockAsync(args);
            //            }
            //            else
            //            {
            //#if DEBUG
            //                var _ = new Core(0, Process.GetCurrentProcess().Id)
            //                       .RunAsync(arg);
            //#endif
            //                return new ShardsCoordinator()
            //                    .RunAndBlockAsync();
            //            }
        //}
        public static void Main(string[] args)
        => MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        /// <summary>
        /// Provides the main way to instaniate the bot
        /// This will have to take in a List using a built in scheduler to make it possible to run multiple bots form one instance
        /// </summary>
        /// <returns></returns>
        public static async Task MainAsync(string[] args)
        {

            var config = new CoreConfig();
            var json = string.Empty;

            if (!File.Exists("config.json"))
            {
                json = JsonConvert.SerializeObject(config);
                File.WriteAllText("config.json", json, new UTF8Encoding(false));
                Console.WriteLine("Config file not found, a new one is generated. Please fill it with the proper values and re run this program");
                Console.ReadKey();

                return;
            }

            json = File.ReadAllText("config.json", new UTF8Encoding(false));
            config = JsonConvert.DeserializeObject<CoreConfig>(json);

            var tasklist = new List<Task>();
            for(var i = 0; i < config.ShardCount+1; i++)
            {
                var bot = new Core(Process.GetCurrentProcess().Id, i);
                tasklist.Add(bot.RunAsync());
                await Task.Delay(7500).ConfigureAwait(false);
            }
            await Task.WhenAll(tasklist).ConfigureAwait(false);

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}
