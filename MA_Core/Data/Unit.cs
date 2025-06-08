using MA_Core.Data.ValueObjects;

namespace MA_Core.Data;

public class Unit
{
    public Unit(UnitCodeName codename)
    {
        Codename = codename;
    }
    
    public UnitCodeName Codename { get; }
    public string IconPath { get; set; } = String.Empty;

    public Level Level1 { get; init; } = new();
    public Level Level2 { get; init; } = new();
    public Level Level3 { get; init; } = new();
    public Level Level4 { get; init; } = new();
    
    public class Level
    {
        public string Name { get; set; } = String.Empty;
        public string SpritePath { get; set; } = String.Empty;
        
        public int HP { get; set; } = 100;
        
        public int Strength { get; set; } = 100;
        public int Toughness { get; set; } = 100;
        public int Precision { get; set; } = 100;
        public int Agility { get; set; } = 100;
        
        public int Power { get; set; } = 100;
        public int Defense { get; set; } = 100;
        public int Aura { get; set; } = 100;
        public int Willpower { get; set; } = 100;
        
        public Action[] Actions { get; init; } = [];
    }
}