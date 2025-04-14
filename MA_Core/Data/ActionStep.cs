namespace MA_Core.Data;

public abstract record ActionStep
{
    public abstract string Description { get; }

    public record SwapPosition : ActionStep
    {
        public override string Description => "Move";
    }
    
    public record PhysicalAttack : ActionStep
    {
        public override string Description => "Does a physical attack";
        
        public int Accuracy { get; init; }
        public int Power { get; init; }
    }

    public abstract record Select : ActionStep
    {
        public record Self : Select
        {
            public override string Description => "Selects the acting unit";
        }
        
        public abstract record Arbitrary : Select
        {
            public bool AllowSelf { get; init; }

            public Enums.Faction Faction { get; init; }
            public Enums.Range Range { get; init; }

            public record Automatic : Arbitrary
            {
                public override string Description => "Automatically selects all eligible units";
            }

            public record Manual : Arbitrary
            {
                public override string Description => "Select units manually";
                
                public int SelectionCount { get; init; }
                public bool UpToSelectionCount { get; init; }
                public bool EmptyFieldAllowed { get; init; }
            }
        }
    }
}