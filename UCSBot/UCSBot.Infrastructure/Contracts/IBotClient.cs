namespace UCSBot.Infrastructure.Contracts;

public interface IBotClient : IDisposable
{
    public Task Start(CancellationToken cancellationToken = default);
    public Task Stop(CancellationToken cancellationToken = default);

    public Task SendMessages(IEnumerable<string> messages, IEnumerable<ulong> channelIds,
        CancellationToken cancellationToken = default);
}