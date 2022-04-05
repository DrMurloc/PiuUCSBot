namespace UCSBot.Domain.Contracts;

public interface IBotClient : IDisposable
{
    public Task Start(CancellationToken cancellationToken = default);
    public Task Stop(CancellationToken cancellationToken = default);

    public Task SendMessages(IEnumerable<string> messages, IEnumerable<ulong> channelIds,
        CancellationToken cancellationToken = default);

    public Task RegisterSlashCommand(string name, string description, Func<ulong, Task<string>> execution);
    public void WhenReady(Func<Task> execution);
}