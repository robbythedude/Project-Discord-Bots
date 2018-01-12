using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace CryptoBot.Modules
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Responds with pong.")]
        public async Task Ping(string echo = "")
        {
            await ReplyAsync("pong");
        }
    }
}
