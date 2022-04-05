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
            await _botClient.RegisterSlashCommand("start-ucs-feed", "Registers the current channel for UCS feeds",
                async channelId =>
                {
                    _logger.LogInformation($"start-ucs-feed command used from channel {channelId}");

                    using var scope = _serviceCollection.CreateScope();

                    var token = new CancellationToken();
                    await scope.ServiceProvider.GetRequiredService<IMediator>()
                        .Send(new RegisterChannelToFeedCommand(channelId, Feed.Ucs), token);

                    return "Attempting to register channel to receive UCS Feed...";
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