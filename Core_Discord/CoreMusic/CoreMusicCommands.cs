using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using System;

namespace Core_Discord.CoreMusic
{
    [Group("music")]
    [Description("Contains all music related commands")]
    public sealed class CoreMusicCommands
    {
        private string requiredRoles;

        [Command("play")]
        [Description("plays music of the given url")]
        public async Task Play(CommandContext e)
        {
            await e.RespondAsync($"Not yet implemented yet").ConfigureAwait(false);
        }
    }
}
