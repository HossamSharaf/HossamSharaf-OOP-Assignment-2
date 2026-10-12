namespace SrpLab;

/// <summary>
/// Loan desk: eligibility arithmetic, required-document lists, and rejection letter prose.
/// </summary>
public record LoanApplication(decimal RequestedAmount, int CreditScore, int EmploymentMonths, bool HasCollateral);

// Reason to change: The underwriting math for scoring financial risk changes.
public class RiskCalculator
{
    public decimal Score(LoanApplication app)
    {
        decimal score = 100m;
        score -= Math.Max(0, 700 - app.CreditScore) * 0.15m;
        if (app.EmploymentMonths < 6) score -= 20m;
        if (app.RequestedAmount > 50_000m && !app.HasCollateral) score -= 25m;
        if (app.RequestedAmount > 150_000m) score -= 10m;
        return Math.Clamp(score, 0m, 100m);
    }

    public bool IsEligible(decimal riskScore, int creditScore) => riskScore >= 55m && creditScore >= 580;
}

// Reason to change: Compliance laws introduce new document requirements.
public class ComplianceChecklist
{
    public IReadOnlyList<string> GetRequiredDocs(LoanApplication app, bool isEligible)
    {
        var docs = new List<string> { "National ID", "Proof of income (3 months)" };
        if (app.RequestedAmount > 40_000m) docs.Add("Bank statements (6 months)");
        if (app.HasCollateral) docs.Add("Collateral ownership deed");
        if (app.EmploymentMonths < 12) docs.Add("Employer letter");
        if (!isEligible) docs.Add("Manual underwriter referral form");
        return docs;
    }
}

// Reason to change: Legal updates the wording of approval or rejection templates.
public class DecisionLetterFormatter
{
    public string Format(string name, decimal requested, decimal score, bool eligible, IReadOnlyList<string> docs)
    {
        if (eligible)
            return $"Dear {name},\nYour request for {requested:C} is pre-approved (risk {score:0}).\nPlease upload: {string.Join("; ", docs)}.\n";
        return $"Dear {name},\nWe are unable to approve {requested:C} at this time.\nReference risk={score:0}. You may reapply after improving documentation.\n";
    }
}

// Reason to change: The data science team requires a new column in the analytics export.
public class UnderwritingCsvEncoder
{
    public string Encode(string appId, LoanApplication app, decimal score, bool eligible) =>
        $"{appId},{app.CreditScore},{app.EmploymentMonths},{(app.HasCollateral ? 1 : 0)},{score:0.00},{(eligible ? "Y" : "N")}";
}