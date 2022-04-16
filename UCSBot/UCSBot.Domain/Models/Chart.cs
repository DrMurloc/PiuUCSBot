namespace UCSBot.Domain.Models;

public sealed record Chart(int ChartId, string SongName, string StepArtistName, string ChartType, int DifficultyLevel,
    DateOnly CreationDate, int StepCount, int HoldCount, int JumpCount,
    int TripleCount, int QuadCount, int QuintPlusCount, int SpeedChangeCount, double LargestSpeedChange,
    bool IsHalfDouble, bool IsQuarterDouble)
{
}