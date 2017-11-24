using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
namespace Core_Discord
{
    public class ShardComMessage
    {
        public int ShardId { get; set; }
        public DiscordConnection ConnectionState { get; set; }
        public int Guilds { get; set; }
        public DateTime Time { get; set; }

        public ShardComMessage Clone() =>
            new ShardComMessage
            {
                ShardId = ShardId,
                ConnectionState = ConnectionState,
                Guilds = Guilds,
                Time = Time,
            };
    }
}
