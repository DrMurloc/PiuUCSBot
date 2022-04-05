using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UCSBot.Application.Events;
using UCSBot.Configurations;
using UCSBot.Domain.Models;
using UCSBot.Dtos;

namespace UCSBot.HostedServices;

public sealed class UcsImportQueueHostedService : IHostedService
{
    private readonly ServiceBusConfiguration _config;
    private readonly ILogger _logger;
    private readonly IServiceProvider _services;
    private ServiceBusClient _client;
    private ServiceBusProcessor _proccessor;

    public UcsImportQueueHostedService(ILogger<UcsImportQueueHostedService> logger,
        IOptions<ServiceBusConfiguration> options,
        IServiceProvider services)
    {
        _logger = logger;
        _config = options.Value;
        _services = services;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client = new ServiceBusClient(_config.ConnectionString);
        _proccessor = _client.CreateProcessor("ucs-imported");
        _proccessor.ProcessErrorAsync += e =>
        {
            _logger.LogError($"Error processing message {e.Exception.Message} {e.Exception.StackTrace}", e);
            return Task.CompletedTask;
        };
        _proccessor.ProcessMessageAsync += async e =>
        {
            var charts = JsonConvert.DeserializeObject<UcsChartImportedDto[]>(e.Message.Body.ToString());
            if (charts == null)
            {
                _logger.LogError($"Could not parse imported charts {e.Message.Body}");
                return;
            }

            using var scope = _services.CreateScope();
            try
            {
                var cancellation = new CancellationToken();
                await scope.ServiceProvider.GetRequiredService<IMediator>()
                    .Publish(new ChartsImportedEvent(charts.Select(c => new Chart(
                        c.ChartId,
                        c.SongName,
                        c.StepArtistName,
                        c.ChartType,
                        c.DifficultyLevel,
                        c.CreationDate,
                        c.StepCount,
                        c.HoldCount,
                        c.JumpCount,
                        c.TripleCount,
                        c.QuadCount,
                        c.QuintPlusCount,
                        c.SpeedChangeCount,
                        c.LargestSpeedChange))), cancellation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error when notifying of chart creation {ex.Message} {ex.StackTrace}", ex);
            }
        };
        await _proccessor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _proccessor.StopProcessingAsync(cancellationToken);
    }
}