using Microsoft.EntityFrameworkCore;
using UCSBot.Domain.Contracts;
using UCSBot.Domain.Models;
using UCSBot.Infrastructure.Persistence;
using UCSBot.Infrastructure.Persistence.Entities;

namespace UCSBot.Infrastructure;

public sealed class SentMessageRepository : ISentMessageRepository
{
    private readonly UcsBotDbContext _database;

    public SentMessageRepository(UcsBotDbContext database)
    {
        _database = database;
    }

    public async Task RecordMessages(IEnumerable<SentChartMessage> messages, CancellationToken cancellationToken)
    {
        var messageArray = messages.ToArray();

        await CreateChartsIfNotExists(messageArray, cancellationToken);

        foreach (var message in messageArray)
            await _database.ChartMessage.AddAsync(new ChartMessageEntity
            {
                Id = message.DiscordId.ToString(),
                ChartId = message.ChartId,
                DiscordId = message.DiscordId
            }, cancellationToken);

        await _database.SaveChangesAsync(cancellationToken);
    }

    private async Task CreateChartsIfNotExists(IEnumerable<SentChartMessage> messages,
        CancellationToken cancellationToken)
    {
        var charts = messages.GroupBy(m => m.ChartId).Select(g => g.First()).ToArray();

        var chartIds = charts.Select(c => c.ChartId).Distinct().ToArray();

        var existingChartIds = (await _database.Chart.Where(c => chartIds.Contains(c.ChartId))
            .Select(c => c.ChartId)
            .ToArrayAsync(cancellationToken)).ToHashSet();

        foreach (var chart in charts.Where(c => !existingChartIds.Contains(c.ChartId)))
            await _database.Chart.AddAsync(new ChartEntity
            {
                ArtistName = chart.Artist,
                ChartId = chart.ChartId,
                ChartType = chart.ChartType,
                CreationDate = chart.CreationDate,
                Level = chart.Level,
                Link = chart.Link,
                SongName = chart.SongName
            }, cancellationToken);
    }
}