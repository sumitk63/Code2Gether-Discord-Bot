﻿using Code2Gether_Discord_Bot.Static;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Code2Gether_Discord_Bot.Modules
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Replies with an embed containing the client's websocket latency
        /// </summary>
        /// <returns></returns>
        [Command("ping")]
        [Alias("pong")]
        public async Task PingAsync() =>
            await ReplyAsync(embed: BusinessLogicFactory.PingLogic(Context, Context.Client.Latency).Execute());
    }
}
