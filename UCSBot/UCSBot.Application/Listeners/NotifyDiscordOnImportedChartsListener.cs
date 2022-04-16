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
    private readonly ISentMessageRepository _sentMessageRepository;

    public NotifyDiscordOnImportedChartsListener(IChannelRepository channelRepository,
        IBotClient botClient, ISentMessageRepository sentMessageRepository)
    {
        _channelRepository = channelRepository;
        _botClient = botClient;
        _sentMessageRepository = sentMessageRepository;
    }

    public async Task Handle(ChartsImportedEvent notification, CancellationToken cancellationToken)
    {
        var channels = (await _channelRepository.GetChannelsSubscribedToFeed(Feed.Ucs, cancellationToken))
            .ToImmutableArray();

        if (!channels.Any()) return;

        var messages = notification.Charts.Select(c => new ChartMessage(c)).ToArray();

        var channelIds = channels.Select(c => c.Id).ToImmutableArray();

        var results = await _botClient.SendMessages(messages, channelIds, cancellationToken);

        await _sentMessageRepository.RecordMessages(results, cancellationToken);
    }
}