using Microsoft.EntityFrameworkCore;
using UCSBot.Domain.Contracts;
using UCSBot.Domain.Enums;
using UCSBot.Domain.Models;
using UCSBot.Infrastructure.Persistence;
using UCSBot.Infrastructure.Persistence.Entities;

namespace UCSBot.Infrastructure;

public sealed class EfChannelRepository : IChannelRepository
{
    private readonly UcsBotDbContext _database;

    public EfChannelRepository(UcsBotDbContext database)
    {
        _database = database;
    }

    public async Task<Channel?> GetChannel(ulong channelId, CancellationToken cancellationToken = default)
    {
        var entity = await _database.Channel.FirstOrDefaultAsync(c => c.ChannelId == channelId, cancellationToken);
        return entity == null ? null : new Channel(entity.ChannelId, entity.FeedNames.Select(Enum.Parse<Feed>));
    }

    public async Task SaveChannel(Channel channel, CancellationToken cancellationToken = default)
    {
        var entity = await _database.Channel.FirstOrDefaultAsync(c => c.ChannelId == channel.Id, cancellationToken);
        if (entity == null)
        {
            entity = new ChannelEntity
            {
                Id = channel.Id.ToString(),
                ChannelId = channel.Id,
                FeedNames = channel.Feeds.Select(f => f.ToString()).ToArray()
            };
            await _database.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.FeedNames = channel.Feeds.Select(f => f.ToString()).ToArray();
            if (!entity.FeedNames.Any())
                _database.Remove(entity);
            else
                _database.Update(entity);
        }

        await _database.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Channel>> GetChannelsSubscribedToFeed(Feed feed,
        CancellationToken cancellationToken = default)
    {
        var feedString = feed.ToString();
        return (await _database.Channel.ToArrayAsync(cancellationToken))
            .Where(c => c.FeedNames.Contains(feedString))
            .Select(c => new Channel(c.ChannelId, c.FeedNames.Select(Enum.Parse<Feed>)))
            .ToArray();
    }
}