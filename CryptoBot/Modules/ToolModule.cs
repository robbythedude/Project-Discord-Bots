using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using CryptoBot.Services;
using CryptoBot.Models;

namespace CryptoBot.Modules
{
    public class ToolModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Responds with pong.")]
        public async Task Ping(string echo = "")
        {
            await ReplyAsync("pong");
        }

        [Command("remindme")]
        [Summary("sets a reminder that will message back when the time comes")]
        public async Task RemindMe(params string[] stringArray)
        {

            if (stringArray.Length < 1)
            {
                await ReplyAsync("Please add a time duration.");

            }
            else if (stringArray.Length < 2)
            {
                await ReplyAsync("Please add a message.");
            }
            else if (stringArray.Length > 2)
            {
                await ReplyAsync("You have too many parameters. Please surround the message in quotes");
            }
            else
            {
                string user = Context.User.Username;
                DateTime currentTime = this.Context.Message.Timestamp.DateTime;
                RemindMeService.InputIntoDB(stringArray, currentTime, user);
                await ReplyAsync("I will remind you when the time comes.");
            }
            
        }
    }
}
