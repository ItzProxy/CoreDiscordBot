using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;

namespace Core_Discord
{
    public sealed class CoreCommands
    {
        /*
         * Function Name: HellowWorld
         * Purpose: Test function
         * 
         * @param CommandContext e
         * @param DiscordUser user
         * 
         * @returnType Task
         * @return "Hello <user> welcome to <serverId>" (string)
         * 
         */
        [Command("helloWorld")]
        public async Task HelloWorld(CommandContext e, DiscordUser user)
        {
            await e.RespondAsync("Hello " + user.Username + ". Welcome to " + e.Guild.Id.ToString()).ConfigureAwait(false);
        }
    }
}
