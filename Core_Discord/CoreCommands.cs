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
        /// <summary>
        /// Return hello <username>user.Username</username>
        /// </summary>
        /// <param name="e"></param>
        /// <param name="user"></param>
        /// <returns> "Hello " <username> user </username> ". Welcome to Guild</returns>
        [Command("helloWorld")]
        public async Task HelloWorld(CommandContext e, DiscordUser user)
        {
            await e.Message.RespondAsync($"{user.Username} Welcome to {user.Discriminator}").ConfigureAwait(false);
        }

        [Command("ping")] // let's define this method as a command
        [Description("Example ping command")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command
        public async Task Ping(CommandContext e) // this command takes no arguments
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await e.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(e.Client, ":ping_pong:");

            // respond with ping
            await e.Message.RespondAsync($"{emoji} Pong! Ping: {e.Client.Ping}ms");
        }

        [Command("join")]
        [Description("Bot joins voice channel if user is in one, otherwise doesn't do anything")]
        [Aliases("jvc")]
        public async Task JoinVC(CommandContext e)
        {
            var server = e.Client(m => )
            await e.Message.RespondAsync($"Command not implemented yet");
        }
    }
}
