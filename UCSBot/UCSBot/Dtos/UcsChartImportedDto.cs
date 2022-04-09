namespace UCSBot.Dtos;

public sealed class UcsChartImportedDto
{
    public int ChartId { get; set; }
    public string SongName { get; set; }
    public string StepArtistName { get; set; }
    public string ChartType { get; set; }
    public int DifficultyLevel { get; set; }
    public DateTime CreationDate { get; set; }
    public int StepCount { get; set; }
    public int HoldCount { get; set; }
    public int JumpCount { get; set; }
    public int TripleCount { get; set; }
    public int QuadCount { get; set; }
    public int QuintPlusCount { get; set; }
    public int SpeedChangeCount { get; set; }
    public double LargestSpeedChange { get; set; }
    public bool IsHalfDouble { get; set; }
    public bool IsQuarterDouble { get; set; }
}