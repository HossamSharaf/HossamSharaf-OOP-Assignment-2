namespace SrpLab;

/// <summary>
/// Subscription billing: proration math, invoice number minting, and dunning email bodies.
/// </summary>
// Reason to change: The data attributes of a customer's subscription lifecycle change.
public class SubscriptionData
{
    public string CustomerId { get; }
    public decimal MonthlyPrice { get; }
    public DateOnly PeriodStart { get; }
    public DateOnly PeriodEnd { get; }
    public int FailedPayments { get; private set; }

    public SubscriptionData(string id, decimal price, DateOnly start, DateOnly end)
    {
        CustomerId = id; MonthlyPrice = price; PeriodStart = start; PeriodEnd = end;
    }
    public void RegisterFailure() => FailedPayments++;
}

// Reason to change: Finance updates the mathematical formula for partial-month billing.
public class ProrationCalculator
{
    public decimal Calculate(decimal price, DateOnly start, DateOnly end, DateOnly activeFrom)
    {
        if (activeFrom <= start) return price;
        if (activeFrom >= end) return 0m;
        var totalDays = end.DayNumber - start.DayNumber;
        if (totalDays <= 0) return price;
        var used = end.DayNumber - activeFrom.DayNumber;
        return Math.Round(price * used / totalDays, 2);
    }
}

// Reason to change: The company's prefix or sequence rules for invoice identifiers change.
public class InvoiceSequence
{
    private int _seq = 1000;
    public string Next(DateOnly periodStart) => $"INV-{periodStart:yyyyMM}-{++_seq:D5}";
}

// Reason to change: The legal or marketing wording for collections emails changes.
public class DunningEmailFormatter
{
    public string Format(string name, string invoiceId, decimal amount, DateOnly asOf, int failures)
    {
        var severity = failures switch { <= 1 => "friendly reminder", 2 => "second notice", _ => "final notice before suspension" };
        return $"Subject: {severity} {invoiceId}\nHi {name},\nBalance {amount:C} as of {asOf:o} ({failures} failures).\n";
    }
}

// Reason to change: The accounting software CSV import schema changes.
public class LedgerCsvEncoder
{
    public string Encode(string customerId, string invoiceId, decimal proratedAmount) =>
        $"{customerId},{invoiceId},{proratedAmount:0.00},AR-SUB";
}
