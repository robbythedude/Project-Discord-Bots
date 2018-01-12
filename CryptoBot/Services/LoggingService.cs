using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CryptoBot.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public LoggingService(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;

            _client.Log += LogAsync;
        }

        private Task LogAsync(LogMessage message)
        {
            return Console.Out.WriteLineAsync(message.ToString());
        }
    }
}
