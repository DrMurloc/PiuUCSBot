using MediatR;
using UCSBot.Domain.Models;

namespace UCSBot.Application.Events;

public sealed class ChartsImportedEvent : INotification
{
    public ChartsImportedEvent(IEnumerable<Chart> charts)
    {
        Charts = charts;
    }

    public IEnumerable<Chart> Charts { get; }
}