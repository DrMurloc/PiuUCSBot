namespace UCSBot.Domain.Models;

public sealed class Chart
{
    public Chart(int chartId, string songName,
        string stepArtistName,
        string chartType,
        int difficultyLevel,
        DateOnly creationDate,
        int stepCount,
        int holdCount,
        int jumpCount,
        int tripleCount,
        int quadCount,
        int quintPlusCount,
        int speedChangeCount,
        double largestSpeedChange)
    {
        ChartId = chartId;
        SongName = songName;
        StepArtistName = stepArtistName;
        ChartType = chartType;
        DifficultyLevel = difficultyLevel;
        CreationDate = creationDate;
        StepCount = stepCount;
        HoldCount = holdCount;
        JumpCount = jumpCount;
        TripleCount = tripleCount;
        QuadCount = quadCount;
        QuintPlusCount = quintPlusCount;
        SpeedChangeCount = speedChangeCount;
        LargestSpeedChange = largestSpeedChange;
    }

    public int ChartId { get; set; }
    public string SongName { get; set; }
    public string StepArtistName { get; set; }
    public string ChartType { get; set; }
    public int DifficultyLevel { get; set; }
    public DateOnly CreationDate { get; set; }
    public int StepCount { get; set; }
    public int HoldCount { get; set; }
    public int JumpCount { get; set; }
    public int TripleCount { get; set; }
    public int QuadCount { get; set; }
    public int QuintPlusCount { get; set; }
    public int SpeedChangeCount { get; set; }
    public double LargestSpeedChange { get; set; }
}