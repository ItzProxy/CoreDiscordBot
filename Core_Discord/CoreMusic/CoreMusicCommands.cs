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
    public sealed class CoreMusicCommands
    {
        private string requiredRoles = DiscordRole;

        [Command("Play")]
        [RequireRolesAttribute()]
    }
}
