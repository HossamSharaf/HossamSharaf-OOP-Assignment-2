namespace SrpLab;

/// <summary>
/// Appointment desk: opening-hours rules, slot search, and ICS calendar blob generation.
/// </summary>
// Reason to change: How the clinic persists or queries its raw booked slots changes.
public class ClinicSchedule
{
    private readonly HashSet<DateTimeOffset> _booked = new();
    public bool IsBooked(DateTimeOffset slot) => _booked.Contains(slot);
    public void Book(DateTimeOffset slot) => _booked.Add(slot);
}

// Reason to change: Clinic HR decides to work on weekends or change their hours.
public class BusinessHoursPolicy
{
    public TimeOnly Open { get; }
    public TimeOnly Close { get; }
    public int SlotMinutes { get; }

    public BusinessHoursPolicy(TimeOnly open, TimeOnly close, int slotMinutes)
    {
        Open = open; Close = close; SlotMinutes = slotMinutes;
    }

    public bool IsWithinHours(DateTimeOffset when)
    {
        if (when.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday) return false;
        var t = TimeOnly.FromDateTime(when.DateTime);
        return t >= Open && t.AddMinutes(SlotMinutes) <= Close;
    }
}

// Reason to change: The search algorithm for finding the next availability changes.
public class SlotFinder
{
    public DateTimeOffset? FindNext(DateTimeOffset from, int searchHours, ClinicSchedule schedule, BusinessHoursPolicy policy)
    {
        var cursor = Align(from, policy.SlotMinutes);
        var end = from.AddHours(searchHours);
        while (cursor < end)
        {
            if (policy.IsWithinHours(cursor) && !schedule.IsBooked(cursor)) return cursor;
            cursor = cursor.AddMinutes(policy.SlotMinutes);
        }
        return null;
    }

    private DateTimeOffset Align(DateTimeOffset from, int slotMins) =>
        new(from.Year, from.Month, from.Day, from.Hour, from.Minute - (from.Minute % slotMins), 0, from.Offset);
}

// Reason to change: RFC specifications for the .ics calendar format are updated.
public class IcsCalendarEncoder
{
    public string Encode(DateTimeOffset slot, int durationMins, string patientName, string clinician)
    {
        var end = slot.AddMinutes(durationMins);
        return "BEGIN:VCALENDAR\nVERSION:2.0\nBEGIN:VEVENT\n" +
               $"UID:{Guid.NewGuid()}\nDTSTART:{slot:yyyyMMdd'T'HHmmss'Z'}\nDTEND:{end:yyyyMMdd'T'HHmmss'Z'}\n" +
               $"SUMMARY:Visit {patientName} / {clinician}\nEND:VEVENT\nEND:VCALENDAR\n";
    }
}

// Reason to change: Clinic marketing wants to adjust the length or tone of SMS reminders.
public class SmsReminderFormatter
{
    public string Format(DateTimeOffset slot, string clinicPhone) =>
        $"Reminder: appointment {slot:MMM dd HH:mm}. Call {clinicPhone} to reschedule.";
}