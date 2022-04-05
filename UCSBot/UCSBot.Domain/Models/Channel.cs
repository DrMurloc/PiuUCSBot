using UCSBot.Domain.Enums;

namespace UCSBot.Domain.Models;

public sealed class Channel
{
    public Channel(ulong id, IEnumerable<Feed> feeds)
    {
        Id = id;
        Feeds = feeds.Distinct().ToHashSet();
    }

    public ulong Id { get; }
    public ISet<Feed> Feeds { get; }

    public void AddFeed(Feed feed)
    {
        if (Feeds.Contains(feed))
            return;
        Feeds.Add(feed);
    }
}