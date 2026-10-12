namespace SrpLab;

/// <summary>
/// Warehouse pick list: naive path order, stock allocation, and picker instruction prose.
/// </summary>
public record PickLine(string Sku, string Aisle, int Bin, int QtyNeeded, int QtyOnHand);

// Reason to change: The definition or state constraints of an active picking order change.
public class PickOrder
{
    private readonly List<PickLine> _lines = new();
    public void AddNeed(string sku, string aisle, int bin, int needed, int onHand) =>
        _lines.Add(new PickLine(sku, aisle, bin, needed, onHand));
    public IReadOnlyList<PickLine> Lines => _lines;
}

// Reason to change: The business rules for allocating partial inventory or backorders change.
public class InventoryAllocator
{
    public IReadOnlyList<(string Sku, int Allocated, int Shortfall)> Allocate(IEnumerable<PickLine> lines) =>
        lines.Select(l => (l.Sku, Math.Min(l.QtyNeeded, l.QtyOnHand), l.QtyNeeded - Math.Min(l.QtyNeeded, l.QtyOnHand))).ToList();
}

// Reason to change: The physical layout of the warehouse dictates a new walking path algorithm.
public class WarehousePathFinder
{
    public IReadOnlyList<PickLine> SortWalkingOrder(IEnumerable<PickLine> lines) =>
        lines.Where(l => Math.Min(l.QtyNeeded, l.QtyOnHand) > 0)
             .OrderBy(l => l.Aisle)
             .ThenBy(l => l.Bin)
             .ToList();
}

// Reason to change: The UI/UX on the handheld scanner devices is redesigned.
public class PickerScriptFormatter
{
    public string Format(IEnumerable<PickLine> path, IEnumerable<(string Sku, int, int Shortfall)> allocations)
    {
        var steps = path.Select((s, i) => $"{i + 1}. Go aisle {s.Aisle} bin {s.Bin}: pick {Math.Min(s.QtyNeeded, s.QtyOnHand)} × {s.Sku}");
        var shortages = allocations.Where(a => a.Shortfall > 0).Select(a => a.Sku).ToList();
        var warn = shortages.Any() ? "SHORTAGES: " + string.Join(", ", shortages) : "SHORTAGES: none";
        return string.Join('\n', steps) + "\n" + warn;
    }
}

// Reason to change: The third-party WMS vendor updates their XML integration schema.
public class WmsBatchEncoder
{
    public string Encode(string batchId, IEnumerable<(string Sku, int Allocated, int)> allocations)
    {
        var parts = allocations.Select(a => $"<line sku=\"{a.Sku}\" qty=\"{a.Allocated}\" />");
        return $"<batch id=\"{batchId}\">{string.Join("", parts)}</batch>";
    }
}