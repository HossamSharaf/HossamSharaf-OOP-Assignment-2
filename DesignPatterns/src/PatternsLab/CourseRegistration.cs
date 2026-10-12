namespace PatternsLab.Problems.Builder;

public sealed class CourseRegistration
{
    public string StudentEmail { get; }
    public string CourseCode { get; }
    public string AccessMode { get; }
    public string? GroupCode { get; }
    public string? DiscountCode { get; }
    public bool SendWhatsApp { get; }
    public bool SendEmailWelcome { get; }
    public string? MentorNote { get; }
    public DateOnly? PreferredStart { get; }

    // Constructor can be made internal/private to force the use of the Builder
    internal CourseRegistration(
        string studentEmail,
        string courseCode,
        string accessMode,
        string? groupCode,
        string? discountCode,
        bool sendWhatsApp,
        bool sendEmailWelcome,
        string? mentorNote,
        DateOnly? preferredStart)
    {
        StudentEmail = studentEmail;
        CourseCode = courseCode;
        AccessMode = accessMode;
        GroupCode = groupCode;
        DiscountCode = discountCode;
        SendWhatsApp = sendWhatsApp;
        SendEmailWelcome = sendEmailWelcome;
        MentorNote = mentorNote;
        PreferredStart = preferredStart;
    }

    public override string ToString()
        => $"{StudentEmail} → {CourseCode} [{AccessMode}] group={GroupCode ?? "-"} discount={DiscountCode ?? "-"} wa={SendWhatsApp} mail={SendEmailWelcome}";
}

// R2: Interfaces define the mandatory steps required before reaching the optional builder steps.
public interface IRegistrationEmailStep
{
    IRegistrationCourseStep WithEmail(string email);
}

public interface IRegistrationCourseStep
{
    IRegistrationAccessStep ForCourse(string courseCode);
}

public interface IRegistrationAccessStep
{
    IRegistrationOptionalSteps WithAccessMode(string accessMode);
}

public interface IRegistrationOptionalSteps
{
    IRegistrationOptionalSteps WithGroupCode(string groupCode);
    IRegistrationOptionalSteps WithDiscountCode(string discountCode);
    IRegistrationOptionalSteps EnableWhatsApp(bool enable = true);
    IRegistrationOptionalSteps EnableEmailWelcome(bool enable = true);
    IRegistrationOptionalSteps WithMentorNote(string note);
    IRegistrationOptionalSteps WithPreferredStart(DateOnly start);
    CourseRegistration Build();
}

// R1: Fluent Builder Implementation
public class CourseRegistrationBuilder :
    IRegistrationEmailStep,
    IRegistrationCourseStep,
    IRegistrationAccessStep,
    IRegistrationOptionalSteps
{
    private string _studentEmail = string.Empty;
    private string _courseCode = string.Empty;
    private string _accessMode = string.Empty;
    private string? _groupCode;
    private string? _discountCode;
    private bool _sendWhatsApp;
    private bool _sendEmailWelcome;
    private string? _mentorNote;
    private DateOnly? _preferredStart;

    private CourseRegistrationBuilder() { }

    public static IRegistrationEmailStep Start() => new CourseRegistrationBuilder();

    public IRegistrationCourseStep WithEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");
        _studentEmail = email;
        return this;
    }

    public IRegistrationAccessStep ForCourse(string courseCode)
    {
        if (string.IsNullOrWhiteSpace(courseCode)) throw new ArgumentException("Course Code is required");
        _courseCode = courseCode;
        return this;
    }

    public IRegistrationOptionalSteps WithAccessMode(string accessMode)
    {
        if (string.IsNullOrWhiteSpace(accessMode)) throw new ArgumentException("Access Mode is required");
        _accessMode = accessMode;
        return this;
    }

    public IRegistrationOptionalSteps WithGroupCode(string groupCode)
    {
        _groupCode = groupCode;
        return this;
    }

    public IRegistrationOptionalSteps WithDiscountCode(string discountCode)
    {
        _discountCode = discountCode;
        return this;
    }

    public IRegistrationOptionalSteps EnableWhatsApp(bool enable = true)
    {
        _sendWhatsApp = enable;
        return this;
    }

    public IRegistrationOptionalSteps EnableEmailWelcome(bool enable = true)
    {
        _sendEmailWelcome = enable;
        return this;
    }

    public IRegistrationOptionalSteps WithMentorNote(string note)
    {
        _mentorNote = note;
        return this;
    }

    public IRegistrationOptionalSteps WithPreferredStart(DateOnly start)
    {
        _preferredStart = start;
        return this;
    }

    // R3: Build() validates rules and throws clear errors before instantiating the object
    public CourseRegistration Build()
    {
        if (_accessMode == "LiveGroup" && string.IsNullOrWhiteSpace(_groupCode))
            throw new InvalidOperationException("Validation Error: LiveGroup requires a GroupCode.");

        if (_accessMode == "VideosOnly" && !string.IsNullOrWhiteSpace(_groupCode))
            throw new InvalidOperationException("Validation Error: VideosOnly cannot have a GroupCode.");

        return new CourseRegistration(
            _studentEmail,
            _courseCode,
            _accessMode,
            _groupCode,
            _discountCode,
            _sendWhatsApp,
            _sendEmailWelcome,
            _mentorNote,
            _preferredStart);
    }
}

// R4: Call sites refactored to use the Step Builder
public static class RegistrationCallSites
{
    public static CourseRegistration CreateLiveStudent()
    {
        return CourseRegistrationBuilder.Start()
            .WithEmail("sara@mail.com")
            .ForCourse("SEF-101")
            .WithAccessMode("LiveGroup")
            .WithGroupCode("G1")
            .WithDiscountCode("EARLY10")
            .EnableWhatsApp()
            .EnableEmailWelcome()
            .WithMentorNote("Needs evening slot")
            .WithPreferredStart(new DateOnly(2026, 10, 1))
            .Build();
    }

    public static CourseRegistration CreateVideosOnly()
    {
        return CourseRegistrationBuilder.Start()
            .WithEmail("ali@mail.com")
            .ForCourse("SEF-101")
            .WithAccessMode("VideosOnly")
            // Not including GroupCode or DiscountCode, completely bypassing the need to pass 'null'
            .EnableEmailWelcome()
            // Not including WhatsApp, allowing it to naturally default to false
            .Build();
    }
}
