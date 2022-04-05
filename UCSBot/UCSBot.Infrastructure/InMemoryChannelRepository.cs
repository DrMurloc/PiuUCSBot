using UCSBot.Domain.Contracts;
using UCSBot.Domain.Models;

namespace UCSBot.Infrastructure;

public sealed class InMemoryChannelRepository : IChannelRepository
{
    private readonly List<Channel> _channels = new();

    public Task<Channel?> GetChannel(ulong channelId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_channels.FirstOrDefault(c => c.Id == channelId));
    }

    public Task SaveChannel(Channel channel, CancellationToken cancellationToken = default)
    {
        _channels.RemoveAll(c => c.Id == channel.Id);
        _channels.Add(channel);
        return Task.CompletedTask;
    }
}