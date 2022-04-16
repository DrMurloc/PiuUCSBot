namespace UCSBot.Domain.Models;

public sealed record ChartMessage(Chart Chart)
{
    public string Message => MessageFromChart(Chart);
    private string Link => $"https://piugame.com/bbs/board.php?bo_table=ucs&wr_id={Chart.ChartId}";

    public SentChartMessage Sent(ulong discordMessageId)
    {
        return new SentChartMessage(discordMessageId, Chart.ChartId, Chart.SongName, Chart.ChartType,
            Chart.DifficultyLevel, Chart.StepArtistName, Link);
    }

    private string MessageFromChart(Chart chart)
    {
        var result = $@"Chart: {chart.SongName} {chart.ChartType} {chart.DifficultyLevel}
Artist: {chart.StepArtistName}
Created on: {chart.CreationDate}";
        if (chart.IsHalfDouble)
        {
            if (chart.IsQuarterDouble)
                result += @"
Quarter Double";
            else
                result += @"
Half Double";
        }

        result += $@"
{chart.StepCount} Steps, {chart.HoldCount} Holds, {chart.JumpCount} Jumps";
        if (chart.TripleCount > 0 || chart.QuadCount > 0 || chart.QuintPlusCount > 0)
        {
            result += @"
";
            if (chart.TripleCount > 0)
                result += $@"{chart.TripleCount} Triples ";

            if (chart.QuadCount > 0)
                result += $@"{chart.QuadCount} Quads ";

            if (chart.QuintPlusCount > 0)
                result += $@"{chart.QuintPlusCount} Quintuple+ Steps";
        }

        if (chart.SpeedChangeCount > 0)
            result += $@"
{chart.SpeedChangeCount} Speed Changes, Maximum Change of {chart.LargestSpeedChange} BPM";

        result += $@"
{Link}";
        return result;
    }
}