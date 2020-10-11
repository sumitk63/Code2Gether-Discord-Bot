﻿using Code2Gether_Discord_Bot.Static;
using Discord.Commands;
using System.Threading.Tasks;

namespace Code2Gether_Discord_Bot.Modules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Replies with a wealth of information regarding the bot's environment such as:
        ///     Author, Library version, Runtime, Uptime, Heap Size, Guilds, Channels, and Users
        /// </summary>
        /// <returns></returns>
        [Command("info")]
        [Alias("about", "whoami", "owner", "uptime", "library", "author", "stats")]
        public async Task InfoAsync() =>
             await ReplyAsync(embed: BusinessLogicFactory.InfoLogic(Context).Execute());
    }
}
