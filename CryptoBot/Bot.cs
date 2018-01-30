﻿using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
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

            //sort the RemindMe file initially
            RemindMeService.SortTextFile();

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
            ulong guildId = UInt64.Parse(config["cryptoServer"]);
            SocketGuild guild = client.GetGuild(guildId);

            string message = RemindMeService.CheckTime(guild);
            if (message != "")
            {
                ulong channelId = UInt64.Parse(config["cryptoChannel"]);
                SocketTextChannel channel= guild.GetTextChannel(channelId);

                channel.SendMessageAsync(message);
            }
        }
    }
}
