using System.Diagnostics;
using PatternsLab.Problems.Builder;
using PatternsLab.Problems.Prototype;
using PatternsLab.Problems.Singleton;

Console.WriteLine("=== SINGLETON: AFTER ===\n");
var db = new DatabaseService();
var ui = new UiService();
Console.WriteLine();

db.Config.Theme = "Dark";
Console.WriteLine("Admin changed theme to Dark.");

// This will now correctly output "Dark" because they share the same instance
ui.Render();

Console.WriteLine($"\nSame config object? {ReferenceEquals(db.Config, ui.Config)}"); // True
Console.WriteLine($"Times config was loaded from disk: {AppConfig.LoadCount}"); // 1


Console.WriteLine("\n=== PROTOTYPE: AFTER ===\n");
var sw = Stopwatch.StartNew();

// 1. Create a single prototype (slow)
var orcPrototype = new Orc();
var army = new List<Enemy>();

// 2. Clone it 5 times (instant)
for (int i = 1; i <= 5; i++)
{
    var orc = orcPrototype.Clone();
    orc.Name = $"Orc-{i}";
    army.Add(orc);
}
Console.WriteLine($"Created 1 prototype and cloned 5 orcs in {sw.ElapsedMilliseconds} ms\n");

Enemy original = new Orc();
original.Name = "Boss Orc";

// Use the new abstract Clone method instead of the ugly switch statement
Enemy copy = original.Clone();

Console.WriteLine($"\nOriginal model id: {original.ModelId}");
Console.WriteLine($"Copy model id:     {copy.ModelId}"); // Now perfectly matches the private state

copy.Weapon.Damage = 999;
Console.WriteLine($"\nWe changed the COPY's weapon damage to 999.");
Console.WriteLine($"Original's weapon damage is now: {original.Weapon.Damage}"); // Remains 25 (Deep Copy!)

copy.Abilities.Add("Fire Breath");
Console.WriteLine($"Original abilities: {string.Join(", ", original.Abilities)}"); // Remains just "Rage"


Console.WriteLine("\n=== BUILDER: AFTER ===\n");

// Using the clean, refactored call sites
Console.WriteLine(RegistrationCallSites.CreateLiveStudent());
Console.WriteLine(RegistrationCallSites.CreateVideosOnly());

// Demonstrating Builder Rule Validation (R3)
Console.WriteLine("\n--- Testing Validation Rules ---");
try
{
    Console.WriteLine("Attempting to build LiveGroup without a GroupCode...");
    CourseRegistrationBuilder.Start()
        .WithEmail("hacker@mail.com")
        .ForCourse("SEF-101")
        .WithAccessMode("LiveGroup")
        // Intentionally skipping .WithGroupCode("...")
        .Build();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Failed securely: {ex.Message}");
}
