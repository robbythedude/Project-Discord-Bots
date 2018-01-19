using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoBot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _services;

        public CommandHandlingService(DiscordSocketClient client, CommandService commands, IConfigurationRoot config, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _config = config;
            _services = services;

            _client.MessageReceived += OnMessageReceived;
        }

        private async Task OnMessageReceived(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;

            if (message != null)
            {
                if (message.Author.Id != _client.CurrentUser.Id)
                {
                    //this is needed for the two methods used in the if statement. Just says where the prefix is located
                    int argPos = 0;
                    char prefix = _config["prefix"].ToCharArray()[0];
                    //using ! as the prefix
                    if (message.HasCharPrefix(prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                    {
                        var context = new SocketCommandContext(_client, message);

                        //gets the result of the command after execution
                        var result = await _commands.ExecuteAsync(context, argPos, _services);

                        if (!result.IsSuccess)
                        {
                            if (result.Error != CommandError.UnknownCommand)
                            {
                                await context.Channel.SendMessageAsync(result.ErrorReason);
                            }
                        }
                    }
                    else if (message.MentionedUsers.Count > 0)
                    {
                        SocketUser user = message.MentionedUsers.FirstOrDefault(u => u.Username == _client.CurrentUser.Username);
                        if (user != null)
                        {
                            var context = new SocketCommandContext(_client, message);

                            await context.Channel.SendMessageAsync("You dare disturb my peace?");
                        }

                    }
                }
            }

            return;
        }
    }
}
