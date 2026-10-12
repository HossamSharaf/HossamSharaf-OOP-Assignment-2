namespace SrpLab;

/// <summary>
/// Support ticket: SLA clocks, NLP-ish priority from free text, and public reply templates.
/// </summary>
// Reason to change: The core fields required to define a support ticket change.
public class SupportTicketData
{
    public string Id { get; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public DateTimeOffset OpenedAt { get; }

    public SupportTicketData(string id, string subject, string body, DateTimeOffset openedAt)
    {
        Id = id; Subject = subject; Body = body; OpenedAt = openedAt;
    }
    public void AppendMessage(string text) => Body += "\n---\n" + text;
}

// Reason to change: Operations updates the NLP heuristics used to guess customer urgency.
public class TicketPriorityScanner
{
    public string Scan(string subject, string body)
    {
        var blob = (subject + " " + body).ToLowerInvariant();
        if (blob.Contains("down") || blob.Contains("outage") || blob.Contains("cannot login")) return "P1";
        if (blob.Contains("urgent") || blob.Contains("asap") || blob.Contains("blocked")) return "P2";
        return "P3";
    }
}

// Reason to change: The company alters its SLA response time guarantees.
public class SlaCalculator
{
    public DateTimeOffset Deadline(DateTimeOffset openedAt, string priority) =>
        openedAt.AddHours(priority switch { "P1" => 4, "P2" => 24, _ => 72 });

    public bool IsBreached(DateTimeOffset deadline, DateTimeOffset now) => now > deadline;
}

// Reason to change: Customer Experience (CX) updates the tone or wording of automated replies.
public class CustomerReplyFormatter
{
    public string Format(string ticketId, string agentName, string priority, DateTimeOffset deadline)
    {
        var apology = priority == "P1" ? "We are treating this as a critical incident." : "Thanks for reaching out.";
        return $"Hi,\n{apology}\nTicket {ticketId} is with {agentName}. Next update before {deadline:u}.\n";
    }
}

// Reason to change: Internal teams change how escalation payloads are structured for Slack/Ops.
public class EscalationFormatter
{
    public string Format(string ticketId, string priority, DateTimeOffset deadline) =>
        $"ESCALATE {ticketId} priority={priority} breachAt={deadline:u} keywords-scanned=yes";
}