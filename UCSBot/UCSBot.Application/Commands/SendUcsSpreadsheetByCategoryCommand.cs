using MediatR;

namespace UCSBot.Application.Commands;

public sealed record SendUcsSpreadsheetByCategoryCommand(ulong UserId, string Category) : IRequest
{
}