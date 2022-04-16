using MediatR;

namespace UCSBot.Application.Commands;

public sealed record CategorizeMessageCommand(ulong MessageId, ulong UserId, string Category) : IRequest
{
}