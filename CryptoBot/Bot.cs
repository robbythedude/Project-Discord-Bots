using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using CryptoBot.Services;
using System.Threading;

namespace CryptoBot
{
    public class Bot
    {
        private DiscordSocketClient client = null;
        private IConfigurationRoot config = null;

        /*allows the program to immidiately start in async mode.*/
        public static void Main(string[] args) => new Bot().MainAsync().GetAwaiter().GetResult();

        /*the main method*/
        public async Task MainAsync()
        {
            config = GetConfiguration();
            
            //construct the "bot"
            client = new DiscordSocketClient();
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

            //start the timer that looks at the file for reminders
            Timer timer = new Timer(CheckIfNeedReminded, "some state", TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

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

        private void CheckIfNeedReminded(object state)
        {
            string message = RemindMeService.CheckTime();
            if (message != "")
            {
                ulong server = UInt64.Parse(config["cryptoServer"]);
                ulong channel = UInt64.Parse(config["cryptoChannel"]);

                client.GetGuild(server).GetTextChannel(channel).SendMessageAsync(message);
            }
        }
    }
}
