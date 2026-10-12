namespace SrpLab;

/// <summary>
/// Hospital ward board: tracks beds, computes acuity scores, drafts nurse handoff notes,
/// and decides which pager code to fire. Looks like "one ward concern" — it is not.
/// </summary>
// Reason to change: The data structures used to track bed assignments and vital scores change.
public class WardCensus
{
    private readonly Dictionary<int, string> _bedPatient = new();
    private readonly Dictionary<int, int> _vitalsScore = new();

    public void Assign(int bed, string patientId, int acuityScore)
    {
        _bedPatient[bed] = patientId.Trim().ToUpperInvariant();
        _vitalsScore[bed] = acuityScore;
    }
    
    public string? GetPatient(int bed) => _bedPatient.GetValueOrDefault(bed);
    public int GetAcuity(int bed) => _vitalsScore.GetValueOrDefault(bed);
    public IEnumerable<int> GetOccupiedBeds() => _bedPatient.Keys.OrderBy(x => x);
}

// Reason to change: The medical/clinical rules for scoring patient health vitals change.
public class AcuityCalculator
{
    public int Score(int heartRate, int spo2)
    {
        var score = 0;
        if (heartRate > 120 || heartRate < 45) score += 4;
        else if (heartRate > 100) score += 2;
        if (spo2 < 90) score += 5;
        else if (spo2 < 94) score += 2;
        return Math.Min(score, 10);
    }
}

// Reason to change: The mechanism for recording or draining emergency alerts changes.
public class WardPagerLog
{
    private readonly List<string> _log = new();
    
    public void RecordCodeYellow(int bed) => 
        _log.Add($"CODE-YELLOW bed={bed} at {DateTime.UtcNow:HH:mm}");
        
    public IReadOnlyList<string> Drain()
    {
        var copy = _log.ToList();
        _log.Clear();
        return copy;
    }
}

// Reason to change: Nurses request a different presentation format for shift handoffs.
public class HandoffNoteFormatter
{
    public string Format(int bed, string? patient, int acuity)
    {
        if (string.IsNullOrEmpty(patient)) return $"Bed {bed}: empty";
        var tone = acuity >= 8 ? "ESCALATE" : acuity >= 4 ? "WATCH" : "STABLE";
        return $"[HANDOFF {DateTime.UtcNow:yyyy-MM-dd}] Bed {bed} · {patient} · acuity={acuity} · {tone}";
    }
}

// Reason to change: IT needs to add, remove, or modify columns in the system data export.
public class CensusCsvEncoder
{
    public string Encode(WardCensus census)
    {
        var lines = new List<string> { "bed,patient,acuity" };
        foreach (var bed in census.GetOccupiedBeds())
            lines.Add($"{bed},{census.GetPatient(bed)},{census.GetAcuity(bed)}");
        return string.Join('\n', lines);
    }
}
