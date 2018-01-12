using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace CryptoBot.Services
{
    class ConnectionService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;


        public ConnectionService(DiscordSocketClient client, CommandService commands, IConfigurationRoot config)
        {
            _client = client;
            _commands = commands;
            _config = config;
        }

        public async Task StartAsync()
        {
            //get the token from the appSettings.json file
            string token = _config["tokens:discord"];

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("The bot's token in the appSettings.json file is not there");
            }

            //connect to discord
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            //load all the modules
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}
