using UCSBot.Domain.Models;

namespace UCSBot.Domain.Contracts;

public interface ISentMessageRepository
{
    Task RecordMessages(IEnumerable<SentChartMessage> messages, CancellationToken cancellationToken);
    Task<SentChartMessage?> GetSentMessage(ulong discordMessageId, CancellationToken cancellationToken = default);

    Task CategorizeMessage(ulong discordUserId, ulong discordMessageId, string category,
        CancellationToken cancellationToken = default);

    Task UnCategorizeMessage(ulong discordUserId, ulong discordMessageId, string category,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SentChartMessage>> GetSentMessagesByCategory(ulong discordUserId, string category,
        CancellationToken cancellationToken = default);
}