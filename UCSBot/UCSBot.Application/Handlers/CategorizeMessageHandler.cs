using MediatR;
using UCSBot.Application.Commands;
using UCSBot.Domain.Contracts;

namespace UCSBot.Application.Handlers;

public sealed class CategorizeMessageHandler : IRequestHandler<CategorizeMessageCommand>
{
    private readonly ISentMessageRepository _repository;

    public CategorizeMessageHandler(ISentMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(CategorizeMessageCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.GetSentMessage(request.MessageId) == null) return Unit.Value;
        await _repository.CategorizeMessage(request.UserId, request.MessageId, request.Category);
        return Unit.Value;
    }
}