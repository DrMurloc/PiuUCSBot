namespace UCSBot.Domain.Models;

public sealed record SentChartMessage(ulong DiscordId, int ChartId, string SongName, string ChartType, int Level,
    string Artist,
    string Link)
{
}