using UCSBot.Domain.Models;

namespace UCSBot.Domain.Contracts;

public interface ISentMessageRepository
{
    Task RecordMessages(IEnumerable<SentChartMessage> messages, CancellationToken cancellationToken);
}