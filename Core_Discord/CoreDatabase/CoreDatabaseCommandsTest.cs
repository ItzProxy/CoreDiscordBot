using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Core_Discord.CoreDatabase
{
    public class CoreDatabaseCommandsTest
    {

        [Command("Setup")]
        [Description("Try to sync with database if it exist")]
        public async Task TestDatabaseConnection(CommandContext e)
        {
            await e.RespondAsync($"I have no idea if this works or not").ConfigureAwait(false);
        }
    }
}
