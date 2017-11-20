//using System.Threading.Tasks;
//using DSharpPlus.CommandsNext;
//using DSharpPlus.CommandsNext.Attributes;
//using DSharpPlus.Entities;
//using DSharpPlus.EventArgs;
//using DSharpPlus.Interactivity;
//using DSharpPlus.VoiceNext;
//using DSharpPlus;
//using System;

//namespace Core_Discord.CoreMusic
//{
//    [Group("music")]
//    [Description("Contains all music related commands")]
//    public partial class CoreMusicPlayer
//    {

//        [Command("play")]
//        [Description("plays music of the given url")]
//        public async Task Play(CommandContext e)
//        {
//            var mp = await _musicService.GetOrCreatePlayer(e);
//            await e.RespondAsync($"Not yet implemented yet").ConfigureAwait(false);
//        }
//    }
//}
