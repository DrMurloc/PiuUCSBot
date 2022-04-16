using MediatR;
using UCSBot.Application.Commands;
using UCSBot.Domain.Contracts;
using UCSBot.Domain.Enums;

namespace UCSBot.HostedServices;

public sealed class BotHostedService : IHostedService
{
    private readonly IBotClient _botClient;
    private readonly ILogger<BotHostedService> _logger;
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceCollection;

    public BotHostedService(IBotClient botClient,
        ILogger<BotHostedService> logger,
        IServiceProvider serviceCollection)
    {
        _botClient = botClient;
        _logger = logger;

        _serviceCollection = serviceCollection;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _botClient.Start(cancellationToken);
        _botClient.WhenReady(async () =>
        {
            _botClient.RegisterReactRemoved(async (emote, userId, messageId) =>
            {
                try
                {
                    using var scope = _serviceCollection.CreateScope();

                    var token = new CancellationToken();
                    await scope.ServiceProvider.GetRequiredService<IMediator>()
                        .Send(new UnCategorizeMessageCommand(messageId, userId, emote), token);
                }
                catch (Exception e)
                {
                    _logger.LogError($"There was an error while removing a reaction: {e.Message} {e.StackTrace}", e);
                }
            });
            _botClient.RegisterReactAdded(async (emote, userId, messageId) =>
            {
                try
                {
                    using var scope = _serviceCollection.CreateScope();

                    var token = new CancellationToken();
                    await scope.ServiceProvider.GetRequiredService<IMediator>()
                        .Send(new CategorizeMessageCommand(messageId, userId, emote), token);
                }
                catch (Exception e)
                {
                    _logger.LogError($"There was an error while adding a reaction: {e.Message} {e.StackTrace}", e);
                }
            });
            await _botClient.RegisterSlashCommand("help", "Bot Quick Start help and Documentation", @"
*Testing bold*
Testing Not Bold", channelId => Task.CompletedTask);
            await _botClient.RegisterSlashCommand("build-ucs-spreadsheet",
                "DMs you a CSV spreadsheet of UCS charts you have reacted to", "Sending UCS Spreadsheet...",
                async (channelId, userId, options) =>
                {
                    try
                    {
                        using var scope = _serviceCollection.CreateScope();

                        var token = new CancellationToken();
                        await scope.ServiceProvider.GetRequiredService<IMediator>()
                            .Send(new SendUcsSpreadsheetByCategoryCommand(userId, options["emote"]),
                                token);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError($"There was an error while adding a reaction: {e.Message} {e.StackTrace}", e);
                    }
                }, new[]
                {
                    ("emote", "The emote react to filter by")
                });
            await _botClient.RegisterSlashCommand("start-ucs-feed", "Registers the current channel for UCS feeds",
                "Attempting to register channel to receive UCS Feed...",
                async channelId =>
                {
                    _logger.LogInformation($"start-ucs-feed command used from channel {channelId}");

                    using var scope = _serviceCollection.CreateScope();

                    var token = new CancellationToken();
                    await scope.ServiceProvider.GetRequiredService<IMediator>()
                        .Send(new RegisterChannelToFeedCommand(channelId, Feed.Ucs), token);
                });

            await _botClient.RegisterSlashCommand("stop-ucs-feed", "UnRegisters the current channel from the UCS feed",
                "Attempting to un-register channel from the UCS Feed...",
                async channelId =>
                {
                    _logger.LogInformation($"stop-ucs-feed command used from channel {channelId}");
                    using var scope = _serviceCollection.CreateScope();

                    var token = new CancellationToken();
                    await scope.ServiceProvider.GetRequiredService<IMediator>()
                        .Send(new UnRegisterChannelFromFeedCommand(channelId, Feed.Ucs), token);
                });
        });
        _logger.LogInformation("Started bot client");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _botClient.Stop(cancellationToken);
        _logger.LogInformation("Stopped bot client");
    }
}