using System.Collections.Immutable;
using MediatR;
using UCSBot.Application.Events;
using UCSBot.Domain.Contracts;
using UCSBot.Domain.Enums;
using UCSBot.Domain.Models;

namespace UCSBot.Application.Listeners;

public sealed class NotifyDiscordOnImportedChartsListener : INotificationHandler<ChartsImportedEvent>
{
    private readonly IBotClient _botClient;
    private readonly IChannelRepository _channelRepository;

    public NotifyDiscordOnImportedChartsListener(IChannelRepository channelRepository,
        IBotClient botClient)
    {
        _channelRepository = channelRepository;
        _botClient = botClient;
    }

    public async Task Handle(ChartsImportedEvent notification, CancellationToken cancellationToken)
    {
        var channels = (await _channelRepository.GetChannelsSubscribedToFeed(Feed.Ucs, cancellationToken))
            .ToImmutableArray();

        if (!channels.Any()) return;

        var messages = notification.Charts.Select(GetChartDescription).ToArray();

        var channelIds = channels.Select(c => c.Id).ToImmutableArray();

        await _botClient.SendMessages(messages, channelIds, cancellationToken);
    }

    private static string GetChartDescription(Chart chart)
    {
        var result = $@"Chart: {chart.SongName} {chart.ChartType} {chart.DifficultyLevel}
Artist: {chart.StepArtistName}
Created on: {chart.CreationDate}";
        result += $@"
{chart.StepCount} Steps, {chart.HoldCount} Holds, {chart.JumpCount} Jumps";
        if (chart.TripleCount > 0 || chart.QuadCount > 0 || chart.QuintPlusCount > 0)
        {
            result += @"
";
            if (chart.TripleCount > 0)
                result += $@"{chart.TripleCount} Triples ";

            if (chart.QuadCount > 0)
                result += $@"{chart.QuadCount} Quads ";

            if (chart.QuintPlusCount > 0)
                result += $@"{chart.QuintPlusCount} Quintuple+ Steps";
        }

        if (chart.SpeedChangeCount > 0)
            result += $@"
{chart.SpeedChangeCount} Speed Changes, Maximum Change of {chart.LargestSpeedChange} BPM";

        result += $@"
https://piugame.com/bbs/board.php?bo_table=ucs&wr_id={chart.ChartId}";
        return result;
    }
}