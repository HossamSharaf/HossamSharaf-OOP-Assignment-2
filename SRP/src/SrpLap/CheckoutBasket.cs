namespace SrpLab;

/// <summary>
/// Online checkout basket: totals, coupon linguistics, gift-wrap copy, and a faux payment auth code.
/// </summary>
// Reason to change: Support for cart item grouping or unique identifiers is added.
public class Cart
{
    private readonly List<(string Sku, decimal Price, int Qty)> _lines = new();

    public void AddLine(string sku, decimal price, int qty)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        _lines.Add((sku, price, qty));
    }
    public IReadOnlyList<(string Sku, decimal Price, int Qty)> Lines => _lines;
}

// Reason to change: New marketing campaigns require parsing different string commands.
public class MarketingCouponParser
{
    public decimal CalculateDiscount(string couponRaw, decimal subTotal)
    {
        if (string.IsNullOrWhiteSpace(couponRaw)) return 0m;
        var t = couponRaw.Trim().ToUpperInvariant();
        if (t.StartsWith("SAVE") && int.TryParse(t[4..], out var pct) && pct is > 0 and <= 50)
            return Math.Round(subTotal * pct / 100m, 2);
        if (t.Contains("FREESHIP")) return 0m;
        if (t == "WELCOME10") return Math.Min(10m, subTotal);
        return 0m;
    }
}

// Reason to change: Packaging rules or flat fee structures change in cart totals.
public class CartPricer
{
    public decimal SubTotal(Cart cart) => cart.Lines.Sum(l => l.Price * l.Qty);

    public decimal GrandTotal(decimal subTotal, decimal discount, bool hasGiftWrap) =>
        Math.Max(0m, subTotal - discount + (hasGiftWrap ? 4.99m : 0m));
}

// Reason to change: The conversational tone or layout of the generated gift cards changes.
public class GiftCardFormatter
{
    public string Format(string fromName, Cart cart, decimal totalSurpriseValue)
    {
        var items = string.Join(", ", cart.Lines.Select(l => l.Sku));
        return $"Dear friend,\nA gift from {fromName} awaits ({items}).\nTotal surprise value: {totalSurpriseValue:C}\n";
    }
}

// Reason to change: The payment gateway mandates a new hashing or payload specification.
public class PaymentPayloadEncoder
{
    public string Encode(decimal amount, string cardLast4, int itemCount)
    {
        var payload = $"{amount:0.00}|{cardLast4}|{itemCount}";
        return $"AUTH-{Math.Abs(payload.GetHashCode()):X8}";
    }
}
