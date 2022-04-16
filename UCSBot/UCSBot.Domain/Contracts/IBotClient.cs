using UCSBot.Domain.Models;

namespace UCSBot.Domain.Contracts;

public interface IBotClient : IDisposable
{
    public Task Start(CancellationToken cancellationToken = default);
    public Task Stop(CancellationToken cancellationToken = default);

    public Task SendMessage(ulong userId, string message, CancellationToken cancellationToken = default);

    public Task SendFile(ulong userId, Stream fileStream, string fileName, string? message = null,
        CancellationToken cancellationToken = default);

    public Task<IEnumerable<SentChartMessage>> SendMessages(IEnumerable<ChartMessage> messages,
        IEnumerable<ulong> channelIds,
        CancellationToken cancellationToken = default);

    public Task SendMessages(IEnumerable<string> messages, IEnumerable<ulong> channelIds,
        CancellationToken cancellationToken = default);

    public Task RegisterSlashCommand(string name, string description, Func<ulong, Task<string>> execution);

    public Task RegisterSlashCommand(string name, string description,
        Func<ulong, ulong, IDictionary<string, string>, Task<string>> execution,
        IEnumerable<(string name, string description)> options);

    public void RegisterReactAdded(Func<string, ulong, ulong, Task> execution);
    public void RegisterReactRemoved(Func<string, ulong, ulong, Task> execution);
    public void WhenReady(Func<Task> execution);
}