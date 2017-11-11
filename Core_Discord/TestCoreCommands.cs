using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

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
        
    }
}
