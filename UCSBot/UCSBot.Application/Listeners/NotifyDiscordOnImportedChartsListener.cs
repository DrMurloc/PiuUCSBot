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

        var messages = notification.Charts.Select(c => new ChartMessage(c)).ToArray();

        var channelIds = channels.Select(c => c.Id).ToImmutableArray();

        await _botClient.SendMessages(messages, channelIds, cancellationToken);
    }
}