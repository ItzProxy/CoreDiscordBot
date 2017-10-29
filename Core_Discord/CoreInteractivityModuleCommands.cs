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
    public sealed class CoreInteractivityModuleCommands : CoreCommandPoint
    {
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
        private async Task Poll(CommandContext e, 
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
            var embed = new DiscordEmbedBuilder{
                Title = "Poll",
                Description = string.Join(" ", pollOptions)
            };

            //send to discord application the embed
            var msg = await e.RespondAsync(embed: embed);

            //add option as reactions
            for(int i = 0; i < options.Length; i++)
            {
                await msg.CreateReactionAsync(options[i]);
            }

            var pollResults = await inter.CollectReactionsAsync(msg, duration);
            var results = pollResults.Reactions.Where(re => options.Contains(re.Key)).Select(re => $"{re.Key}: {re.Value}");

            //post results
            await e.RespondAsync(String.Join("/n", results));
        }
    }
}
