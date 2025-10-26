using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ImmerseYourselfServer;

public class DiscordBotManager
{
    private readonly DiscordSocketClient client;
    private readonly CommandService commands;
    private const string token = Constants.DISCORD_API_TOKEN;
    
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService service;

        public InfoModule(CommandService service)
        {
            this.service = service;
        }
        
        [Command("help")]
        [Summary("Shows a list of commands.")]
        public Task HelpAsync()
        {
            var builder = new EmbedBuilder()
                .WithTitle("Available Commands")
                .WithColor(Color.Blue);

            foreach (var command in service.Commands)
            {
                var commandName = command.Name;
                var summary = command.Summary ?? "No description provided";
            
                builder.AddField(x =>
                {
                    x.Name = $"!{commandName}";
                    x.Value = summary;
                    x.IsInline = false;
                });
            }

            return ReplyAsync(embed: builder.Build());
        }
        
        [Command("ping")]
        [Summary("Tells you if there is a mini game currently running or not.")]
        public Task PingAsync()
        {
            var builder = new EmbedBuilder()
                .WithTitle("Ping result")
                .WithColor(Color.Blue);

            builder.AddField(x =>
            {
                x.Name = $"Is currently playing?";
                x.Value = Server.gameData.isPlaying ? ":white_check_mark: Yes" : ":x: No";
                x.IsInline = false;
            });

            return ReplyAsync(embed: builder.Build());
        }
        
        [Command("next")]
        [Summary("Forces the next mini game to happen.")]
        public Task NextAsync()
        {
            return ReplyAsync("Next mini game to happen.");
        }
    }

    public DiscordBotManager()
    {
        this.client = new DiscordSocketClient();
        this.commands = new CommandService();
        Task.Run(InstallCommandsAsync);
        Task.Run(StartBotAsync);
    }

    public async Task StartBotAsync()
    {
        this.client.Log += message =>
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        };
        await this.client.LoginAsync(TokenType.Bot, token);
        await this.client.StartAsync();
        await Task.Delay(-1);
    }
    
    public async Task InstallCommandsAsync()
    {
        client.MessageReceived += MessageHandler;

        await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
            services: null);
    }

    private async Task MessageHandler(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null || message.Author.IsBot)
            return;
        int argPos = 0;
        if (!message.HasCharPrefix('!', ref argPos))
            return;
        var context = new SocketCommandContext(client, message);
        
        await commands.ExecuteAsync(
            context: context, 
            argPos: argPos,
            services: null);
        // await ReplyAsync(message, "C# response works!");
    }

    private async Task ReplyAsync(SocketMessage message, string response)
    {
        await message.Channel.SendMessageAsync(response);
    }
}