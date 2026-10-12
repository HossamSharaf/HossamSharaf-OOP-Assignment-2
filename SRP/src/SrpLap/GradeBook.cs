namespace SrpLab;

/// <summary>
/// Grade book: aggregation, letter-band policy, and transcript export.
/// </summary>
// Reason to change: The internal data structures for tracking raw student assignment scores change.
public class StudentScores
{
    private readonly Dictionary<string, List<decimal>> _scores = new(StringComparer.OrdinalIgnoreCase);
    public void Record(string id, decimal score)
    {
        if (score is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(score));
        if (!_scores.TryGetValue(id, out var list)) _scores[id] = list = new List<decimal>();
        list.Add(score);
    }
    public IReadOnlyList<decimal> Get(string id) => _scores.GetValueOrDefault(id) ?? new List<decimal>();
    public IEnumerable<string> GetStudents() => _scores.Keys.OrderBy(x => x);
}

// Reason to change: The mathematical formula (e.g., median instead of mean) for aggregating scores changes.
public class ScoreAverager
{
    public decimal Calculate(IReadOnlyList<decimal> scores) =>
        scores.Count == 0 ? 0m : Math.Round(scores.Average(), 2);
}

// Reason to change: The academic faculty senate changes grading bands or honor roll thresholds.
public class AcademicPolicy
{
    public string GetLetter(decimal avg) =>
        avg >= 90 ? "A" : avg >= 80 ? "B" : avg >= 70 ? "C" : avg >= 60 ? "D" : "F";

    public bool IsHonorRoll(decimal avg) => avg >= 85 && GetLetter(avg) is "A" or "B";
}

// Reason to change: The physical formatting/layout of plain-text transcripts changes.
public class TranscriptFormatter
{
    public string Format(string studentId, string fullName, decimal avg, string letter, bool honor) =>
        $"TRANSCRIPT\nStudent: {fullName} ({studentId})\nAverage: {avg}\nLetter: {letter}\nHonor: {honor}\n";
}

// Reason to change: The IT registrar system requires a new CSV header or layout.
public class GradeCsvEncoder
{
    public string EncodeHeader() => "studentId,average,letter,honor";
    public string EncodeRow(string studentId, decimal average, string letter, bool honor) =>
        $"{studentId},{average},{letter},{(honor ? 1 : 0)}";
}
