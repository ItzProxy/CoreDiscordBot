using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System.Threading.Tasks;
using System.Globalization;

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
    }
}
