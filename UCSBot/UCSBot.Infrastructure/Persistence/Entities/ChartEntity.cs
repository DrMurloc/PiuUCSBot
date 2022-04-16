using System.ComponentModel.DataAnnotations;

namespace UCSBot.Infrastructure.Persistence.Entities;

public sealed class ChartEntity
{
    [Key] public int ChartId { get; set; }

    [Required] public string SongName { get; set; }

    [Required] public string ArtistName { get; set; }

    [Required] public string ChartType { get; set; }

    [Required] public int Level { get; set; }

    [Required] public string Link { get; set; }

    [Required] public DateOnly CreationDate { get; set; }
}