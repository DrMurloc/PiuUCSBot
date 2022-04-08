using MediatR;
using UCSBot.Domain.Enums;

namespace UCSBot.Application.Commands;

public sealed class UnRegisterChannelFromFeedCommand : IRequest
{
    public UnRegisterChannelFromFeedCommand(ulong channelId, Feed feed)
    {
        ChannelId = channelId;
        Feed = feed;
    }

    public ulong ChannelId { get; }
    public Feed Feed { get; }
}