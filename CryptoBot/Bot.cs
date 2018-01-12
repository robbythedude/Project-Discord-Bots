using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using CryptoBot.Services;

namespace CryptoBot
{
    public class Bot
    {
        /*allows the program to immidiately start in async mode.*/
        public static void Main(string[] args) => new Bot().MainAsync().GetAwaiter().GetResult();

        /*the main method*/
        public async Task MainAsync()
        {
            var config = GetConfiguration();
            
            //construct the "bot"
            var client = new DiscordSocketClient();
            var commands = new CommandService();

            var services = new ServiceCollection()
                        .AddSingleton(client)
                        .AddSingleton(commands)                        
                        .AddSingleton(config)
                        .AddSingleton<ConnectionService>()
                        .AddSingleton<CommandHandlingService>()
                        .AddSingleton<LoggingService>()
                        .BuildServiceProvider();

            //initialize the logging service
            services.GetRequiredService<LoggingService>();
            //connect to discord
            await services.GetRequiredService<ConnectionService>().StartAsync();
            //initialize the command handling service
            services.GetRequiredService<CommandHandlingService>();
            //block this task until the program is closed
            await Task.Delay(-1);
        }

        /*
         * Builds the configuration object
         */
        private IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appSettings.json").Build();
        }
    }
}
