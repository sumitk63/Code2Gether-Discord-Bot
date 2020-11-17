﻿using Code2Gether_Discord_Bot.Static;
using Discord.Commands;
using System.Threading.Tasks;

namespace Code2Gether_Discord_Bot.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Replies with a wealth of information regarding the bot's environment such as:
        ///     Author, Library version, Runtime, Uptime, Heap Size, Guilds, Channels, and Users
        /// </summary>
        /// <returns></returns>
        [Command("help",
            RunMode = RunMode.Async)]
        [Summary("Returns this!")]
        public async Task HelpAsync() =>
             await ReplyAsync(embed: BusinessLogicFactory.HelpLogic(GetType(), Context).Execute());
    }
}