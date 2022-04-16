using System.ComponentModel.DataAnnotations;

namespace UCSBot.Infrastructure.Persistence.Entities;

public sealed class ChartMessageEntity
{
    public string Id { get; set; }

    [Required] [Key] public ulong DiscordId { get; set; }

    [Required] public int ChartId { get; set; }
}