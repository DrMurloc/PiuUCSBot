using UCSBot.Domain.Models;

namespace UCSBot.Domain.Contracts;

public interface IChannelRepository
{
    Task<Channel?> GetChannel(ulong channelId, CancellationToken cancellationToken = default);

    Task SaveChannel(Channel channel, CancellationToken cancellationToken = default);
}