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
    public class CoreCommands
    {
        /// <summary>
        /// Return hello <username>user.Username</username>
        /// </summary>
        /// <param name="e"></param>
        /// <param name="user"></param>
        /// <returns> "Hello " <username> user </username> ". Welcome to Guild</returns>
        [Command("hello")]
        [Description("Says hello")]
        public async Task HelloWorld(CommandContext e, 
            [Description("the mentioned user")] DiscordUser user = null)
        {
            await e.TriggerTypingAsync();
            if(user == null)
            {
                user = e.User;
            }
            var server = e.Guild.Name;
            var emoji = DiscordEmoji.FromName(e.Client, ":wave:");

            await e.Message.RespondAsync($"{emoji} Hello, {user.Mention}! Welcome to {server}").ConfigureAwait(false);
        }

        public async Task HelloWorld(CommandContext e)
        {
            await e.TriggerTypingAsync();

            var server = e.Guild.Name;
            var user = e.Member.Mention;
            var emoji = DiscordEmoji.FromName(e.Client, ":wave:");

            await e.Message.RespondAsync($"{emoji} Hello, {user}! Welcome to {server}").ConfigureAwait(false);
        }
        /// <summary>
        /// When called gets the ping of the Discord Client to the Discord bot
        /// </summary>
        /// <param name="e"></param>
        /// <returns> <emoji>wave</emoji> </returns>
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
        /// <summary>
        /// Checks if user mentioned voice channel or not, then proceeds to check if the channel specified is a voice channel
        /// if no channel specified, gets the current voice channel that the user is in and if the user is in one joins it
        /// </summary>
        /// <param name="e"></param>
        /// <param name="chan"></param>
        /// <returns> Bot joins voice channel: User bound or Argument bound </returns>
        [Command("join")]
        [Description("Bot joins voice channel if user is in one, otherwise doesn't do anything")]
        [Aliases("jvc")]
        public async Task JoinVC(CommandContext e, DiscordChannel chan = null)
        {
            await e.TriggerTypingAsync();
            var vnext = e.Client.GetVoiceNextClient();
            if(vnext == null)
            {
                await e.RespondAsync("VoiceNextService not enabled or configured");
                return;
            }

            //
            var voiceConn = vnext.GetConnection(e.Guild);
            if(voiceConn != null)
            {
                await e.RespondAsync("Already connected in this guild");
                return;
            }

            //check if user is in a voice channel
            var voiceState = e.Member?.VoiceState;
            if(voiceState?.Channel == null && chan == null)
            {
                await e.RespondAsync("You are not in a voice channel");
                return;
            }

            //if channel not specified
            if(chan == null) {
                chan = voiceState.Channel;
            }

            voiceConn = await vnext.ConnectAsync(chan).ConfigureAwait(false);
            await e.RespondAsync($"Connected to '{chan.Name}'");
        }
        [Command("disconnect")]
        [Description("Disconnect bot from current voice channel")]
        [Aliases("dc")]
        public async Task DisconnectVC(CommandContext e)
        {
            await e.RespondAsync($"Not implemented").ConfigureAwait(false);
        }
        [Command("leave")]
        [Description("Bot leaves voice channel")]
        [Aliases("bye")]
        public async Task Leave(CommandContext e)
        {
            await e.RespondAsync($"Not implemented").ConfigureAwait(false);
        }
        [Command("invite")]
        [Description("Create invite link")]
        [Aliases("invlink")]
        public async Task Invite(CommandContext e)
        {
            await e.RespondAsync($"Not implemented").ConfigureAwait(false);
        }
        [Command("Roll")]
        [Description("Roll a multi dimensional dice - roll <max number of sides>")]
        public async Task Roll(CommandContext e)
        {
            var arg = e.RawArgumentString;

           await  e.RespondAsync($"here: ({arg})").ConfigureAwait(false);
        }


        /// <summary>
        /// Taken from DsharpPlus Example Bot 3
        /// </summary>
        /// <param name="e"></param>
        /// <param name="duration"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        ///
        [Command("poll")]
        [Description("Creates a poll for users to use")]
        public async Task Poll(CommandContext e,
            [Description("Poll title/question")] string title,
            [Description("How long should this poll last for?")] TimeSpan duration,
            [Description("What are the options to chose from")] params DiscordEmoji[] options)
        {
            //get the interactivity module from client
            var inter = e.Client.GetInteractivityModule();
            ///set poll options from parameters
            var pollOptions = options.Select(op => op.ToString());

            //new instance of discord embed builder
            //Title - Poll
            //Field = pollOptions array
            var embed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = string.Join(" ", pollOptions)
            };

            embed.WithFooter(
                "Time before over = " + duration
                );
            //send to discord application the embed
            var msg = await e.RespondAsync(embed: embed);

            //add option as reactions
            for (int i = 0; i < options.Length; i++)
            {
                await msg.CreateReactionAsync(options[i]);
            }

            var pollResults = await inter.CollectReactionsAsync(msg, duration);
            var results = pollResults.Reactions.Where(re => options.Contains(re.Key)).Select(re => $"{re.Key}: {re.Value}");

            //post results
            await e.RespondAsync(String.Join("The results/n", results));
        }
    }
}
