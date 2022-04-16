using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UCSBot.Domain.Contracts;
using UCSBot.Domain.Models;
using UCSBot.Infrastructure.Configuration;

namespace UCSBot.Infrastructure;

public sealed class DiscordBotClient : IBotClient
{
    private readonly DiscordConfiguration _configuration;
    private readonly ILogger _logger;
    private DiscordSocketClient? _client;

    public DiscordBotClient(ILogger<DiscordBotClient> logger, IOptions<DiscordConfiguration> options)
    {
        _logger = logger;
        _configuration = options.Value;
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        if (_client != null) throw new Exception("Discord client was already started");

        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info
        });

        _client.Log += msg =>
        {
            _logger.LogInformation(msg.Message);
            return Task.CompletedTask;
        };
        await _client.LoginAsync(TokenType.Bot, _configuration.BotToken);
        await _client.StartAsync();
    }

    public async Task Stop(CancellationToken cancellationToken = default)
    {
        if (_client == null) throw new Exception("Client was never started");
        await _client.StopAsync();
        await _client.DisposeAsync();
    }

    public async Task SendMessage(ulong userId, string message, CancellationToken cancellationToken = default)
    {
        var user = await GetUser(userId);

        await user.SendMessageAsync(message);
    }

    public async Task<IEnumerable<SentChartMessage>> SendMessages(IEnumerable<ChartMessage> messages,
        IEnumerable<ulong> channelIds, CancellationToken cancellationToken = default)
    {
        var result = new List<SentChartMessage>();
        await SendMessages(messages, channelIds, m => m.Message,
            (chart, response) => { result.Add(chart.Sent(response.Id)); });
        return result;
    }

    public async Task SendMessages(IEnumerable<string> messages, IEnumerable<ulong> channelIds,
        CancellationToken cancellationToken = default)
    {
        await SendMessages(messages, channelIds, m => m);
    }

    public async Task RegisterSlashCommand(string name, string description, Func<ulong, Task<string>> execution)
    {
        await RegisterSlashCommand(name, description, o => { }, async command => await execution(command.Channel.Id));
    }

    public void WhenReady(Func<Task> execution)
    {
        if (_client == null) throw new Exception("Client was not started");
        _client.Ready += execution;
    }

    public void RegisterReactRemoved(Func<string, ulong, ulong, Task> execution)
    {
        if (_client == null) throw new Exception("Bot was not initialized");
        _client.ReactionRemoved += async (message, channel, reaction) =>
        {
            await execution(reaction.Emote.ToString() ?? string.Empty, reaction.UserId, reaction.MessageId);
        };
    }

    public void RegisterReactAdded(Func<string, ulong, ulong, Task> execution)
    {
        if (_client == null) throw new Exception("Bot was not initialized");
        _client.ReactionAdded += async (message, channel, reaction) =>
        {
            await execution(reaction.Emote.ToString() ?? string.Empty, reaction.UserId, reaction.MessageId);
        };
    }

    public async Task SendFile(ulong userId, Stream fileStream, string fileName, string? message = null,
        CancellationToken cancellationToken = default)
    {
        var user = await GetUser(userId);

        await user.SendFileAsync(fileStream, fileName, message);
    }

    public async Task RegisterSlashCommand(string name, string description,
        Func<ulong, ulong, IDictionary<string, string>, Task<string>> execution,
        IEnumerable<(string name, string description)> options)
    {
        await RegisterSlashCommand(name, description, builder =>
        {
            foreach (var option in options)
            {
                var optionBuilder = new SlashCommandOptionBuilder()
                    .WithName(option.name)
                    .WithDescription(option.description)
                    .WithType(ApplicationCommandOptionType.String)
                    .WithRequired(true);
                builder.AddOption(optionBuilder);
            }
        }, async command =>
        {
            return await execution(command.Channel.Id, command.User.Id,
                command.Data.Options.ToDictionary(o => o.Name, o => o.Value.ToString() ?? string.Empty));
        });
    }

    private async Task RegisterSlashCommand(string name, string description, Action<SlashCommandBuilder> builderOptions,
        Func<SocketSlashCommand, Task<string>> execution)
    {
        if (_client == null) throw new Exception("Discord client was not started");
        var builder = new SlashCommandBuilder()
            .WithName(name)
            .WithDescription(description);
        builderOptions(builder);

        try
        {
            await _client.CreateGlobalApplicationCommandAsync(builder.Build());
            _client.SlashCommandExecuted += async command =>
            {
                if (command.CommandName == name)
                    try
                    {
                        var response = await execution(command);
                        await command.RespondAsync(response);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(
                            $"An exception while executing the command for {command.CommandName}: {e.Message} {e.StackTrace}",
                            e);
                    }
            };
        }
        catch (Exception e)
        {
            _logger.LogError($"Error when registering the slash command {name}", e);
        }
    }

    private async Task<IUser> GetUser(ulong userId)
    {
        if (_client == null) throw new Exception("Client was never started");

        var user = await _client.GetUserAsync(userId);
        if (user == null) throw new Exception($"User {userId} was not found when sending a message");

        return user;
    }

    private async Task SendMessages<T>(IEnumerable<T> messageEntities, IEnumerable<ulong> channelIds,
        Func<T, string> messageRetrieval,
        Action<T, IUserMessage>? process = default)
    {
        var messageArray = messageEntities.ToArray();

        if (_client == null) throw new Exception("Client was never started");
        foreach (var channelId in channelIds)
        {
            if (await _client.GetChannelAsync(channelId) is not IMessageChannel channel)
            {
                _logger.LogWarning($"Channel {channelId} was not found");
                continue;
            }

            foreach (var message in messageArray)
                try
                {
                    var userMessage = await channel.SendMessageAsync(messageRetrieval(message));
                    if (process != null) process(message, userMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Could not send message to channel {channelId}. Message :{message}", ex);
                }
        }
    }
}