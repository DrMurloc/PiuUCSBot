using System.Collections;
using System.Globalization;
using CsvHelper;
using MediatR;
using UCSBot.Application.Commands;
using UCSBot.Domain.Contracts;

namespace UCSBot.Application.Handlers;

public sealed class SendUcsSpreadsheetByCategoryHandler : IRequestHandler<SendUcsSpreadsheetByCategoryCommand>
{
    private readonly IBotClient _botClient;
    private readonly ISentMessageRepository _sentMessageRepository;

    public SendUcsSpreadsheetByCategoryHandler(ISentMessageRepository sentMessageRepository,
        IBotClient botClient)
    {
        _sentMessageRepository = sentMessageRepository;
        _botClient = botClient;
    }

    public async Task<Unit> Handle(SendUcsSpreadsheetByCategoryCommand request, CancellationToken cancellationToken)
    {
        var messages =
            (await _sentMessageRepository.GetSentMessagesByCategory(request.UserId, request.Category,
                cancellationToken)).OrderBy(m => m.CreationDate).ToArray();
        if (!messages.Any())
        {
            await _botClient.SendMessage(request.UserId, $"You have no chart messages marked with {request.Category}",
                cancellationToken);
            return Unit.Value;
            ;
        }

        var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync((IEnumerable)messages.Select(m => new
        {
            m.SongName,
            m.ChartType,
            m.Level,
            m.Artist,
            m.CreationDate,
            m.Link
        }), cancellationToken);

        await writer.FlushAsync();
        await _botClient.SendFile(request.UserId, writer.BaseStream, $"UCS-Charts-{request.Category}.csv",
            $"UCS Charts you reacted with {request.Category}",
            cancellationToken);

        return Unit.Value;
    }
}