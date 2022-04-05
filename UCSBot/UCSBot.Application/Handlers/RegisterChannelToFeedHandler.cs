using MediatR;
using UCSBot.Application.Commands;
using UCSBot.Domain.Contracts;
using UCSBot.Domain.Enums;
using UCSBot.Domain.Models;

namespace UCSBot.Application.Handlers;

public sealed class RegisterChannelToFeedHandler : IRequestHandler<RegisterChannelToFeedCommand>
{
    private readonly IBotClient _botClient;
    private readonly IChannelRepository _channelRepository;

    public RegisterChannelToFeedHandler(IBotClient botClient,
        IChannelRepository channelRepository)
    {
        _botClient = botClient;
        _channelRepository = channelRepository;
    }

    public async Task<Unit> Handle(RegisterChannelToFeedCommand request, CancellationToken cancellationToken)
    {
        var feedDescription = request.Feed.GetDescription();

        var channel = await _channelRepository.GetChannel(request.ChannelId, cancellationToken) ??
                      new Channel(request.ChannelId, Array.Empty<Feed>());

        if (channel.Feeds.Contains(request.Feed))
        {
            await _botClient.SendMessages(
                new[] { $"This channel already is registered to receive the {feedDescription}" }, new[] { channel.Id },
                cancellationToken);
            return Unit.Value;
        }

        channel.AddFeed(request.Feed);

        await _channelRepository.SaveChannel(channel, cancellationToken);

        await _botClient.SendMessages(
            new[] { $"This channel has been registered to receive the {feedDescription}!" }, new[] { channel.Id },
            cancellationToken);

        return Unit.Value;
    }
}