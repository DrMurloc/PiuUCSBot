using MediatR;

namespace UCSBot.Application.Commands;

public sealed record UnCategorizeMessageCommand(ulong MessageId, ulong UserId, string Category) : IRequest
{
}