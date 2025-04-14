namespace MA_Core.Data;

public class Unit
{
    public string Codename { get; set; }
    public string IconPath { get; set; }

    public Level Level1 { get; set; }
    public Level Level2 { get; set; }
    public Level Level3 { get; set; }
    public Level Level4 { get; set; }
    
    public class Level
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public string SpritePath { get; set; }
        
        public int HP { get; set; }
        
        public int Strength { get; set; }
        public int Toughness { get; set; }
        public int Precision { get; set; }
        public int Agility { get; set; }
        
        public int Power { get; set; }
        public int Defense { get; set; }
        public int Aura { get; set; }
        public int Willpower { get; set; }
        
        public Action[] Actions { get; set; }
    }
}