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

    public async Task<SentChartMessage?> GetSentMessage(ulong discordMessageId,
        CancellationToken cancellationToken = default)
    {
        var cm = await _database.ChartMessage.FirstOrDefaultAsync(cm => cm.DiscordId == discordMessageId,
            cancellationToken);
        if (cm == null) return null;

        var chart = await _database.Chart.SingleAsync(c => c.ChartId == cm.ChartId, cancellationToken);
        return new SentChartMessage(cm.DiscordId, chart.ChartId, chart.SongName, chart.ChartType, chart.Level,
            chart.ArtistName, DateOnly.FromDateTime(chart.CreationDate));
    }

    public async Task CategorizeMessage(ulong discordUserId, ulong discordMessageId, string category,
        CancellationToken cancellationToken = default)
    {
        var existingEntity = await _database.UserMessageCategory.FirstOrDefaultAsync(
            umc => umc.UserId == discordUserId && umc.MessageId == discordMessageId && umc.Category == category,
            cancellationToken);
        if (existingEntity != null) return;

        await _database.UserMessageCategory.AddAsync(new UserMessageCategoryEntity
        {
            Category = category,
            MessageId = discordMessageId,
            UserId = discordUserId
        }, cancellationToken);

        await _database.SaveChangesAsync(cancellationToken);
    }

    public async Task UnCategorizeMessage(ulong discordUserId, ulong discordMessageId, string category,
        CancellationToken cancellationToken = default)
    {
        var existingEntity = await _database.UserMessageCategory.Where(
                umc => umc.UserId == discordUserId && umc.MessageId == discordMessageId && umc.Category == category)
            .ToArrayAsync(cancellationToken);

        if (!existingEntity.Any()) return;

        _database.UserMessageCategory.RemoveRange(existingEntity);
        await _database.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SentChartMessage>> GetSentMessagesByCategory(ulong discordUserId, string category,
        CancellationToken cancellationToken = default)
    {
        var messageIds = await _database.UserMessageCategory
            .Where(umc => umc.UserId == discordUserId && umc.Category == category).Select(umc => umc.MessageId)
            .ToArrayAsync(cancellationToken);
        if (!messageIds.Any()) return Array.Empty<SentChartMessage>();

        var messageCharts = await _database.ChartMessage.Where(cm => messageIds.Contains(cm.DiscordId))
            .ToDictionaryAsync(
                cm => cm.DiscordId, cm => cm.ChartId, cancellationToken);

        if (!messageCharts.Any()) return Array.Empty<SentChartMessage>();

        var chartIds = messageCharts.Values.ToArray();

        var charts = await _database.Chart.Where(c => chartIds.Contains(c.ChartId))
            .ToDictionaryAsync(c => c.ChartId, cancellationToken);

        return messageCharts.Select(mc => new SentChartMessage(mc.Key, mc.Value, charts[mc.Value].SongName,
            charts[mc.Value].ChartType, charts[mc.Value].Level, charts[mc.Value].ArtistName,
            DateOnly.FromDateTime(charts[mc.Value].CreationDate)));
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
                CreationDate = chart.CreationDate.ToDateTime(TimeOnly.MinValue),
                Level = chart.Level,
                SongName = chart.SongName
            }, cancellationToken);
    }
}