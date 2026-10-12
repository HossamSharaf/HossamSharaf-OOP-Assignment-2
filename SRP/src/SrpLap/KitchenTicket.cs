namespace SrpLab;

/// <summary>
/// Kitchen ticket: allergen scan from ingredients, cook ETA heuristics, and printed ticket layout.
/// </summary>
public record KitchenItem(string Name, List<string> Ingredients, int PrepMinutes);

// Reason to change: The way kitchen orders are batched or stored changes.
public class KitchenOrder
{
    private readonly List<KitchenItem> _items = new();
    public void Add(string item, IEnumerable<string> ingredients, int prep) =>
        _items.Add(new KitchenItem(item, ingredients.Select(i => i.Trim().ToLowerInvariant()).ToList(), prep));
    public IReadOnlyList<KitchenItem> Items => _items;
}

// Reason to change: The FDA updates definitions of what constitutes a dietary allergen.
public class AllergenDetector
{
    public IReadOnlyList<string> Detect(IEnumerable<KitchenItem> items)
    {
        var hits = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var ing in items.SelectMany(i => i.Ingredients))
        {
            if (ing.Contains("milk") || ing.Contains("cheese") || ing.Contains("butter")) hits.Add("dairy");
            if (ing.Contains("wheat") || ing.Contains("flour") || ing.Contains("bread")) hits.Add("gluten");
            if (ing.Contains("peanut") || ing.Contains("almond") || ing.Contains("cashew")) hits.Add("nuts");
            if (ing.Contains("shrimp") || ing.Contains("prawn") || ing.Contains("crab")) hits.Add("shellfish");
        }
        return hits.OrderBy(x => x).ToList();
    }
}

// Reason to change: Station throughput capacity math or parallel operations change.
public class PrepTimeEstimator
{
    public int Estimate(IEnumerable<KitchenItem> items, int stations, bool hasAllergies)
    {
        if (stations <= 0) stations = 1;
        if (!items.Any()) return 0;
        var parallel = (int)Math.Ceiling(items.Sum(i => i.PrepMinutes) / (double)stations);
        if (hasAllergies) parallel += 3;
        return Math.Max(parallel, items.Max(i => i.PrepMinutes));
    }
}

// Reason to change: A new receipt printer is purchased requiring different character boundaries.
public class ThermalTicketFormatter
{
    public string Format(int orderNumber, IEnumerable<KitchenItem> items, int eta, IReadOnlyList<string> allergens)
    {
        var line = new string('=', 32);
        var body = string.Join('\n', items.Select(i => $"* {i.Name.ToUpperInvariant()} ({i.PrepMinutes}m)"));
        var allergyLine = allergens.Count == 0 ? "ALLERGENS: none" : "ALLERGENS: " + string.Join(",", allergens);
        return $"{line}\nORDER #{orderNumber}\nETA {eta} MIN\n{body}\n{allergyLine}\n{line}\n";
    }
}

// Reason to change: The physical layout of dispatch lanes in the kitchen is reorganized.
public class ExpoLaneRouter
{
    public string Route(bool hasAllergies, int eta) =>
        hasAllergies ? "LANE-ALLERGY" : eta > 20 ? "LANE-SLOW" : "LANE-FAST";
}
