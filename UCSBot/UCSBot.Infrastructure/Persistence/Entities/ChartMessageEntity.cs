using System.ComponentModel.DataAnnotations;

namespace UCSBot.Infrastructure.Persistence.Entities;

public sealed class ChartMessageEntity
{
    public string Id => DiscordId.ToString();

    [Required] [Key] public ulong DiscordId { get; set; }

    [Required] public int ChartId { get; set; }
}