namespace MA_Core.Data;

public abstract class ActionStep
{
    public abstract string Description { get; }

    public class SwapPosition : ActionStep
    {
        public override string Description => "Move";
    }
    
    public class PhysicalAttack : ActionStep
    {
        public override string Description => "Does a physical attack";
        
        public int Accuracy { get; set; }
        public int Power { get; set; }
    }

    public abstract class Select : ActionStep
    {
        public class Self : Select
        {
            public override string Description => "Selects the acting unit";
        }
        
        public abstract class Arbitrary : Select
        {
            public bool AllowSelf { get; set; }

            public Enums.Faction Faction { get; set; }
            public Enums.Range Range { get; set; }

            public class Automatic : Arbitrary
            {
                public override string Description => "Automatically selects all eligible units";
            }

            public class Manual : Arbitrary
            {
                public override string Description => "Select units manually";
                
                public int SelectionCount { get; set; }
                public bool UpToSelectionCount { get; set; }
                public bool EmptyFieldAllowed { get; set; }
            }
        }
    }
}