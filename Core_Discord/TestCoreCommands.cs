using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System.Globalization;
using DSharpPlus.Entities;

namespace Core_Discord
{
    public class TestCoreCommands
    {
        [Command("DoNothing")]
        [Description("Does nothing")]
        public async Task Nothing(CommandContext e)
        {
            await e.RespondAsync("Does nothing");
        }

        [Command("interChar")]
        [Description("testing out if we can parse floats, int and char")]
        [Aliases("ic")]
        public async Task testIntFloatChar(CommandContext e)
        {
            string result;
            var interactivity = e.Client.GetInteractivity();
            await e.RespondAsync("Awaiting response...");
            var msg = interactivity.WaitForMessageAsync(x => (char.TryParse(x.Content.ToLower(), out var value) && Char.IsLetter(value)),TimeSpan.FromSeconds(60));
            switch (Convert.ToChar(msg.Result.Message.Content))
            {
                case 'i':
                    result = "you have selected i";
                    break;
                case 'b':
                    result = "you have selected b";
                    break;
                default:
                    result = "Not a valid response";
                    break;
                    
            }
            await e.RespondAsync($"The result is: {result}");
            await Task.CompletedTask;
        }
        [Command("interFloat")]
        [Description("testing out if we can parse floats, int and char")]
        [Aliases("if")]
        public async Task testFloat (CommandContext e)
        {
            string result;
            var interactivity = e.Client.GetInteractivity();
            await e.RespondAsync("Awaiting response...");
            var msg = interactivity.WaitForMessageAsync(x => x.Content == x.Content, TimeSpan.FromSeconds(60));
            if(float.TryParse(msg.Result.Message.Content,out var res))
            {
                result = res.ToString();
            }
            else
            {
                result = "Not a float";
            }
            await e.RespondAsync($"The result is: {result}");
            await Task.CompletedTask;
        }
        [Group("bind"), Description("Various argument binder testing commands.")]
        public class Binding
        {
            [Command("user"), Description("Attempts to get a user.")]
            public Task UserAsync(CommandContext ctx, DiscordUser usr)
                => ctx.RespondAsync(embed: new DiscordEmbedBuilder().WithDescription(usr.Mention));

            [Command("member"), Description("Attempts to get a member.")]
            public Task MemberAsync(CommandContext ctx, DiscordMember mbr)
                => ctx.RespondAsync(embed: new DiscordEmbedBuilder().WithDescription(mbr.Mention));

            [Command("role"), Description("Attempts to get a role.")]
            public Task RoleAsync(CommandContext ctx, DiscordRole rol)
                => ctx.RespondAsync(embed: new DiscordEmbedBuilder().WithDescription(rol.Mention));

            [Command("channel"), Description("Attempts to get a channel.")]
            public Task ChannelAsync(CommandContext ctx, DiscordChannel chn)
                => ctx.RespondAsync(embed: new DiscordEmbedBuilder().WithDescription(chn.Mention));

            [Command("guild"), Description("Attempts to get a guild.")]
            public Task GuildAsync(CommandContext ctx, DiscordGuild gld)
                => ctx.RespondAsync(embed: new DiscordEmbedBuilder().WithDescription(gld.Name));

            [Command("emote"), Description("Attempts to get an emoji.")]
            public Task EmoteAsync(CommandContext ctx, DiscordEmoji emt)
                => ctx.RespondAsync(embed: new DiscordEmbedBuilder().WithDescription(emt.ToString()));

            [Command("string"), Description("Attempts to bind a string.")]
            public Task StringAsync(CommandContext ctx, string s)
                => ctx.RespondAsync(s);
        }
    }
}
