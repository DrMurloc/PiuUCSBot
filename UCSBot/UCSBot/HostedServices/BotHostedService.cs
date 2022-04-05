using UCSBot.Infrastructure.Contracts;

namespace UCSBot.HostedServices;

public sealed class BotHostedService : IHostedService
{
    private readonly IBotClient _botClient;
    private readonly ILogger<BotHostedService> _logger;

    public BotHostedService(IBotClient botClient,
        ILogger<BotHostedService> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _botClient.Start(cancellationToken);
        _logger.LogInformation("Started bot client");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _botClient.Stop(cancellationToken);
        _logger.LogInformation("Stopped bot client");
    }
}