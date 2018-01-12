using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CryptoBot
{
    public class Bot
    {
        //initialize the main objects that the bot needs to be aware of in all methods
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        /*allows the program to immidiately start in async mode.*/
        public static void Main(string[] args) => new Bot().MainAsync().GetAwaiter().GetResult();

        /*the main method*/
        public async Task MainAsync()
        {
            //construct the "bot"
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                        .AddSingleton(_client)
                        .AddSingleton(_commands)
                        .BuildServiceProvider();


            await ActivateCommands();

            //connect to discord
            string token = "NDAxMTM1OTQyOTkzMzEzNzky.DTl-OA.yk8zSWgyRr0iwLiSI11VsMN7gJE";
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Log += Log;
            //_client.MessageReceived += MessageReceived;

          


            //block this task until the program is closed
            await Task.Delay(-1);
        }

        public async Task ActivateCommands()
        {
            _client.MessageReceived += HandleCommand;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommand(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            if (message != null)
            {
                int argPos = 0;
                if (message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var context = new SocketCommandContext(_client, message);

                    var result = await _commands.ExecuteAsync(context, argPos, _services);

                    if (!result.IsSuccess)
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }
        }

        /*
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
        }*/


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
