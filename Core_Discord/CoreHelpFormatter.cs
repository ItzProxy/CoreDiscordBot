﻿
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;

namespace Core_Discord
{  
    /// <summary>
    /// Executes each method in a top down form when parsing any command using IHelpFormatter Interface
    /// </summary>
    public sealed class CoreBotHelpFormatter : IHelpFormatter
    {
        private string _name = null, _desc = null, _args = null, _aliases = null, _subcs = null;
        private bool _gexec = false;

        //borrowed from DSharpPlus Test Cases
        /// <summary>
        /// Gets the command name from attribute
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IHelpFormatter WithCommandName(string name)
        {
            this._name = name;
            return this;
        }
        /// <summary>
        /// Gets the description attribute
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public IHelpFormatter WithDescription(string description)
        {
            this._desc = string.IsNullOrWhiteSpace(description) ? null : description;
            return this;
        }

        public IHelpFormatter WithGroupExecutable()
        {
            this._gexec = true;
            return this;
        }
        /// <summary>
        /// Gets the alias attribute
        /// </summary>
        /// <param name="aliases"></param>
        /// <returns></returns>
        public IHelpFormatter WithAliases(IEnumerable<string> aliases)
        {
            if (aliases.Any())
                this._aliases = string.Join(", ", aliases);
            return this;
        }
        /// <summary>
        /// Gets number of arguments from method
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public IHelpFormatter WithArguments(IEnumerable<CommandArgument> arguments)
        {
            if (arguments.Any())
                this._args = string.Join(", ", arguments.Select(xa => $"{xa.Name}: {xa.Type.ToUserFriendlyName()}"));
            return this;
        }
        /// <summary>
        /// Puts together all of the above methods into a string to be packaged out
        /// </summary>
        /// <param name="subcommands"></param>
        /// <returns></returns>
        public IHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            if (subcommands.Any())
            {
                var ml = subcommands.Max(xc => xc.Name.Length);
                var sb = new StringBuilder();
                foreach (var xc in subcommands)
                    sb.Append(xc.Name.PadRight(ml, ' '))
                        .Append("  ")
                        .Append(string.IsNullOrWhiteSpace(xc.Description) ? "No description." : xc.Description).Append("\n");
                this._subcs = sb.ToString();
            }
            return this;
        }
        /// <summary>
        /// Builds the help menu using which takes all types of commands found in associated event
        /// In Core.cs - provides the event hooks
        /// </summary>
        /// <returns></returns>
        public CommandHelpMessage Build()
        {
            var sb = new StringBuilder();
            sb.Append("```less\n");
            if (this._name == null)
            {
                if (this._subcs != null)
                    sb.Append("Displaying all available commands.\n\n");
                else
                    sb.Append("No commands are available.");
            }
            else
            {
                sb.Append(this._name).Append("\n\n");

                if (this._gexec)
                    sb.Append("This group can be executed as a standalone command.\n\n");

                if (this._desc != null)
                    sb.Append("Description: ").Append(this._desc).Append("\n");

                if (this._args != null)
                    sb.Append("Arguments:   ").Append(this._args).Append("\n");

                if (this._aliases != null)
                    sb.Append("Aliases:     ").Append(this._aliases).Append("\n");

                if (this._subcs != null)
                    sb.Append("Subcommands:\n\n");
            }

            if (this._subcs != null)
                sb.Append(_subcs);
            else
                sb.Append("\n");

            sb.Append("```");
            return new CommandHelpMessage(sb.ToString());
        }
    }
}
