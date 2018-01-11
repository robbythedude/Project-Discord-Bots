using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace CryptoBot
{
    public class Bot
    {
        /*allows the program to immidiately start in async mode.*/
        public static void Main(string[] args) => new Bot().MainAsync().GetAwaiter().GetResult();


        /*the main method*/
        public async Task MainAsync()
        {
            var client = new DiscordSocketClient();

            client.Log += Log;
            client.MessageReceived += MessageReceived;

            //connect to discord
            string token = "";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();


            //block this task until the program is closed
            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }

            else if (message.Content == "!BTC")
            {
                await message.Channel.SendMessageAsync("$0 Bitcoin is SHIT");
            }
        }


        /*
         * A method to handle Discord.Net's log events. Can use any logging framework here.
         */
        private Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
    }
}
