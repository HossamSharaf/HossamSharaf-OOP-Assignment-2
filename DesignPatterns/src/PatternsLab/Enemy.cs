namespace PatternsLab.Problems.Prototype;

public class Weapon
{
    public string Name { get; set; }
    public int Damage { get; set; }

    public Weapon Clone() => new Weapon { Name = Name, Damage = Damage };
}

public abstract class Enemy
{
    private string _modelData; // R3: Will be copied via MemberwiseClone

    public string Name { get; set; }
    public int Health { get; set; }
    public Weapon Weapon { get; set; }
    public List<string> Abilities { get; set; } = new();
    public string ModelId => _modelData;

    protected Enemy()
    {
        Console.WriteLine($"   ...loading 3D model for {GetType().Name} (slow)...");
        Thread.Sleep(500);
        _modelData = "MODEL_" + Guid.NewGuid().ToString("N")[..6];
    }

    // R2: Abstract base class provides the cloning mechanism
    public virtual Enemy Clone()
    {
        // R1 & R3: MemberwiseClone creates a shallow copy, capturing private fields 
        // without invoking the slow constructor again.
        var clone = (Enemy)this.MemberwiseClone();

        // R4: Deep copy the reference types so mutations are independent
        clone.Weapon = this.Weapon.Clone();
        clone.Abilities = new List<string>(this.Abilities);

        return clone;
    }
}

public class Orc : Enemy
{
    public Orc()
    {
        Name = "Orc";
        Health = 100;
        Weapon = new Weapon { Name = "Axe", Damage = 25 };
        Abilities.Add("Rage");
    }
}

public class Elf : Enemy
{
    public Elf()
    {
        Name = "Elf";
        Health = 70;
        Weapon = new Weapon { Name = "Bow", Damage = 18 };
        Abilities.Add("Stealth");
    }
}

// R5 (Bonus): A registry that stores named prototypes and returns clones on demand
public class EnemyRegistry
{
    private readonly Dictionary<string, Enemy> _prototypes = new();

    public void Register(string key, Enemy prototype)
    {
        _prototypes[key] = prototype;
    }

    public Enemy Spawn(string key)
    {
        return _prototypes[key].Clone();
    }
}
