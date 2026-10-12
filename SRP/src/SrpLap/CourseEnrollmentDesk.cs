namespace SrpLab;

/// <summary>
/// Course enrollment: capacity, waitlist math, welcome-packet markdown, and invoice lines.
/// </summary>
// Reason to change: The memory models used to store seats and waiting lists change.
public class CourseRoster
{
    public int Capacity { get; }
    public string CourseCode { get; }
    public HashSet<string> Seated { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Waitlist { get; } = new();

    public CourseRoster(string code, int capacity)
    {
        CourseCode = code; Capacity = capacity;
    }
}

// Reason to change: The operational rules for how students move from waitlist to seated change.
public class EnrollmentProcessor
{
    public string Register(CourseRoster roster, string email)
    {
        email = email.Trim();
        if (roster.Seated.Contains(email) || roster.Waitlist.Contains(email)) return "ALREADY_REGISTERED";
        if (roster.Seated.Count < roster.Capacity)
        {
            roster.Seated.Add(email);
            return "SEATED";
        }
        roster.Waitlist.Add(email);
        return $"WAITLIST:{roster.Waitlist.Count}";
    }

    public void PromoteWaitlist(CourseRoster roster, int openSeats)
    {
        while (openSeats > 0 && roster.Waitlist.Count > 0 && roster.Seated.Count < roster.Capacity)
        {
            roster.Seated.Add(roster.Waitlist[0]);
            roster.Waitlist.RemoveAt(0);
            openSeats--;
        }
    }

    public int GetWaitlistPosition(CourseRoster roster, string email) =>
        roster.Waitlist.FindIndex(x => x.Equals(email, StringComparison.OrdinalIgnoreCase)) + 1;
}

// Reason to change: Marketing redesigns the Markdown layout of student welcome material.
public class WelcomePacketFormatter
{
    public string Format(string courseCode, string studentName, bool isSeated, int waitPosition)
    {
        var status = isSeated ? "confirmed seat" : $"waitlist #{waitPosition}";
        return $"# Welcome to {courseCode}\nHi {studentName},\nYour status: **{status}**.\nBring a laptop. Discord onboarding link: https://example.invalid/{courseCode.ToLowerInvariant()}\n";
    }
}

// Reason to change: Local tax authorities change VAT laws or invoice formatting requirements.
public class TuitionInvoiceFormatter
{
    public string Format(string courseCode, decimal tuition, bool isSeated)
    {
        if (!isSeated) return $"{courseCode},WAITLIST,0.00";
        var vat = Math.Round(tuition * 0.14m, 2);
        return $"{courseCode},TUITION,{tuition:0.00},VAT,{vat:0.00},TOTAL,{(tuition + vat):0.00}";
    }
}
