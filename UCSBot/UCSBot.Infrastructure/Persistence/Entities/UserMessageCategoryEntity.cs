using System.ComponentModel.DataAnnotations;

namespace UCSBot.Infrastructure.Persistence.Entities;

public sealed class UserMessageCategoryEntity
{
    [Required] public ulong UserId { get; set; }

    [Required] public ulong MessageId { get; set; }

    [Required] public string Category { get; set; }
}