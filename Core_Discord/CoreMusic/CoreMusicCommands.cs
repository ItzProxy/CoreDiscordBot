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
    public partial class CoreMusicPlayer
    {
        private string requiredRoles;

        [Command("play")]

        [Description("plays music of the given url")]
        public async Task Play(CommandContext e, )
        {
            var mp = await _musicService.
            await e.RespondAsync($"Not yet implemented yet").ConfigureAwait(false);
        }
    }
}
