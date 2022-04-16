using MediatR;
using UCSBot.Application.Commands;
using UCSBot.Domain.Contracts;

namespace UCSBot.Application.Handlers;

public sealed class UnCategorizeMessageHandler : IRequestHandler<UnCategorizeMessageCommand>
{
    private readonly ISentMessageRepository _repository;

    public UnCategorizeMessageHandler(ISentMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UnCategorizeMessageCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.GetSentMessage(request.MessageId) == null) return Unit.Value;
        await _repository.UnCategorizeMessage(request.UserId, request.MessageId, request.Category);
        return Unit.Value;
    }
}