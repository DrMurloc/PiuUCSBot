using MediatR;
using UCSBot.Application.Commands;
using UCSBot.Domain.Contracts;
using UCSBot.Domain.Enums;

namespace UCSBot.Application.Handlers;

public sealed class UnRegisterChannelFromFeedHandler : IRequestHandler<UnRegisterChannelFromFeedCommand>
{
    private readonly IBotClient _bot;
    private readonly IChannelRepository _channels;

    public UnRegisterChannelFromFeedHandler(IChannelRepository channels,
        IBotClient bot)
    {
        _channels = channels;
        _bot = bot;
    }

    public async Task<Unit> Handle(UnRegisterChannelFromFeedCommand request, CancellationToken cancellationToken)
    {
        var channel = await _channels.GetChannel(request.ChannelId, cancellationToken);
        if (channel == null || !channel.Feeds.Contains(request.Feed))
        {
            await _bot.SendMessages(
                new[] { $"This channel is not registered to the {request.Feed.GetDescription()}" },
                new[] { request.ChannelId }, cancellationToken);
            return Unit.Value;
        }

        channel.RemoveFeed(request.Feed);
        await _channels.SaveChannel(channel, cancellationToken);
        await _bot.SendMessages(new[]
        {
            $"Channel removed from the {request.Feed.GetDescription()}"
        }, new[] { request.ChannelId }, cancellationToken);
        return Unit.Value;
    }
}