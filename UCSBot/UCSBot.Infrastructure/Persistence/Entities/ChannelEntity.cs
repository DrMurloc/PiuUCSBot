using System.ComponentModel.DataAnnotations;

namespace UCSBot.Infrastructure.Persistence.Entities;

public sealed class ChannelEntity
{
    public string Id { get; set; }

    [Key] public ulong ChannelId { get; set; }

    [Required] public string[] FeedNames { get; set; } = Array.Empty<string>();
}