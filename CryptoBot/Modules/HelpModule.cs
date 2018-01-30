using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using CryptoBot.Services;

namespace CryptoBot.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("A way to test the response of a bot.")]
        public async Task Help(string echo = "")
        {
            await ReplyAsync("Real men don't need help.");
        }
    }
}
