using MediatR;
using UCSBot.Domain.Enums;

namespace UCSBot.Application.Commands;

public sealed class RegisterChannelToFeedCommand : IRequest
{
    public RegisterChannelToFeedCommand(ulong channelId, Feed feed)
    {
        ChannelId = channelId;
        Feed = feed;
    }

    public ulong ChannelId { get; }
    public Feed Feed { get; }
}