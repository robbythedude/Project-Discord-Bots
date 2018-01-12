using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;


namespace CryptoBot
{
    public class Bot
    {
        //initialize the main objects that the bot needs to be aware of in all methods.
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        /*allows the program to immidiately start in async mode.*/
        public static void Main(string[] args) => new Bot().MainAsync().GetAwaiter().GetResult();

        /*the main method*/
        public async Task MainAsync()
        {

            var config = GetConfiguration();
            //construct the "bot"
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                        .AddSingleton(_client)
                        .AddSingleton(_commands)
                        .BuildServiceProvider();


            //add the commands we built to he service
            await ActivateCommands();

            //connect to discord
            string token = config["tokens:discord"];
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Log += Log;
            //_client.MessageReceived += MessageReceived

            //block this task until the program is closed
            await Task.Delay(-1);
        }

        /*
         * This method hooks into the MessageReceived Event and discovers all our commands and loads them
         */
        public async Task ActivateCommands()
        {
            //hook messages to our command handler
            _client.MessageReceived += HandleCommand;

            //finds all our commands in this current assembly and loads them
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }


        /*
         * Takes a message, checks if it is a command and then processes it
         */
        private async Task HandleCommand(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;

            if (message != null)
            {
                //this is needed for the two methods used in the if statement. Just says where the prefix is located
                int argPos = 0;

                //using ! as the prefix
                if (message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {
                    var context = new SocketCommandContext(_client, message);

                    //gets the result of the command after execution
                    var result = await _commands.ExecuteAsync(context, argPos, _services);

                    if (!result.IsSuccess)
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }

            return;
        }

        /*
         * Builds the configuration object
         */
        private IConfigurationRoot GetConfiguration()
        {

            return new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appSettings.json").Build();
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
