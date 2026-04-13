namespace AlfaSyncDashboard.Models;

public sealed class ExecutionProgress
{
    public string LocalDescripcion { get; set; } = string.Empty;
    public string Etapa { get; set; } = string.Empty;
    public int StageIndex { get; set; }
    public int TotalStages { get; set; }
    public int OverallPercent { get; set; }
    public TimeSpan Elapsed { get; set; }
    public TimeSpan EstimatedRemaining { get; set; }
    public string Message { get; set; } = string.Empty;
}
