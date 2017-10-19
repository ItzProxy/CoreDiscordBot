using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core_Discord
{
    internal sealed class Program
    {
        List<Thread> KeepRunning;
        public static void Main(string[] arg)
        {
            try
            {
                
                MainAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an exception: {ex.ToString()}");
            }
        }
        /// <summary>
        /// Provides the 
        /// </summary>
        /// <returns></returns>
        public static async Task MainAsync()
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
            for(var i = 0; i < config.ShardCount; i++)
            {
                var bot = new Core(config, i);
                tasklist.Add(bot.RunAsync());
            }
            await Task.WhenAll(tasklist).ConfigureAwait(false);

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}
