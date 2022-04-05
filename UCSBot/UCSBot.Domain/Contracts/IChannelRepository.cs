using UCSBot.Domain.Enums;
using UCSBot.Domain.Models;

namespace UCSBot.Domain.Contracts;

public interface IChannelRepository
{
    Task<Channel?> GetChannel(ulong channelId, CancellationToken cancellationToken = default);

    Task SaveChannel(Channel channel, CancellationToken cancellationToken = default);
    Task<IEnumerable<Channel>> GetChannelsSubscribedToFeed(Feed feed, CancellationToken cancellationToken = default);
}