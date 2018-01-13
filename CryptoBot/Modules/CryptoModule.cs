using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using CryptoBot.Services;

namespace CryptoBot.Modules
{
    public class CryptoModule : ModuleBase<SocketCommandContext>
    {

        [Command("price")]
        [Summary("Gets back price of a crypto.")]
        public async Task Price([Remainder]string crypto)
        {
            string message = await RESTService.GetPrice(crypto);
            await ReplyAsync($"The price of {crypto} is: $ {message}");
        }

        [Command("binance")]
        [Summary("Gets back price of a crypto.")]
        public async Task Binance([Remainder]string crypto)
        {
            string message = await RESTService.GetBinancePrice(crypto);
            await ReplyAsync($"The price of {crypto} on binance is: $ {message}");
        }
    }
}
