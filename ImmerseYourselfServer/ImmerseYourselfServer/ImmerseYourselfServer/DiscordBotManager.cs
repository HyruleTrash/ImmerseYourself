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
    private Dictionary<string, ISocketMessageChannel> registeredChannels = new ();
    
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
        
        [Command("start")]
        [Summary("starts the game. (you *could* call !next instead)")]
        public Task StartAsync()
        {
            return NextAsync();
        }
        
        [Command("next")]
        [Summary("Forces the next mini game to happen.")]
        public Task NextAsync()
        {
            MiniGames? game = Server.StartGame();
            
            var builder = new EmbedBuilder()
                .WithTitle("Triggering next mini game")
                .WithColor(Color.Blue);
            
            if (game != null)
                builder.AddField(x =>
                {
                    x.Name = "Next mini game to happen:";
                    x.Value = $"{game}";
                    x.IsInline = false;
                });
            else
                builder.AddField(x =>
                {
                    x.Name = "No more minigames available.";
                    x.Value = "Reset and let all minigames be played again? Enter command !reset OR !resetnext";
                    x.IsInline = false;
                });
            
            return ReplyAsync(embed: builder.Build());
        }
        
        [Command("reset")]
        [Summary("Resets the available mini games.")]
        public Task ResetAsync()
        {
            Server.gameData.playedMiniGamesTemp.Clear();
            Console.WriteLine("playedMiniGamesTemp has been cleared. by discord user.");
            
            var builder = new EmbedBuilder()
                .WithTitle("Reset result")
                .WithColor(Color.Orange);
            builder.AddField(x =>
            {
                x.Name = "Server will now repeat minigames in random order.";
                x.Value = "(controls will no longer be explained)";
                x.IsInline = false;
            });
            
            return ReplyAsync(embed: builder.Build());
        }
        
        [Command("resetnext")]
        [Summary("Resets the available mini games. AND Forces the next mini game to happen.")]
        public Task ResetNextAsyncCommand()
        {
            return ResetNextAsync();
        }

        private async Task ResetNextAsync()
        {
            await ResetAsync();
            await NextAsync();
        }
        
        [Command("end")]
        [Summary("Shuts down the server.")]
        public Task EndAsyncCommand()
        {
            return EndAsync();
        }
        
        private async Task EndAsync()
        {
            await SendServerEndMessageAsync();
            await EndServerAsync();
        }

        private Task SendServerEndMessageAsync()
        {
            Console.WriteLine("Server has been stopped. by discord user.");
            var builder = new EmbedBuilder()
                .WithTitle("Server shut down received")
                .WithColor(Color.Red);
            builder.AddField(x =>
            {
                x.Name = "Shutting down...";
                x.Value = ":dove:";
                x.IsInline = false;
            });
            return ReplyAsync(embed: builder.Build());
        }
        
        private Task EndServerAsync()
        {
            ServerProgram.isRunning = false;
            return Task.CompletedTask;
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
        
        registeredChannels.TryAdd(messageParam.Author.AvatarId, message.Channel);
        
        await commands.ExecuteAsync(
            context: context, 
            argPos: argPos,
            services: null);
    }

    public void AnnounceMessageAsync(string title, string header, string context)
    {
        foreach (var (user, messageChannel) in registeredChannels)
        {
            Task.Run(async () =>
            {
                var builder = new EmbedBuilder()
                    .WithTitle(title)
                    .WithColor(Color.Red);
                builder.AddField(x =>
                {
                    x.Name = header;
                    x.Value = context;
                    x.IsInline = false;
                });
                return messageChannel.SendMessageAsync(embed: builder.Build());
            });
        }
    }
}