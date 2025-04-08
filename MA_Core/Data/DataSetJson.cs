using MA_Core.Data.Enums;

namespace MA_Core.Data;

/// <summary>
/// Diese Klasse wird für das Serialisieren und Deserialisieren von Datasets verwendet
/// </summary>
public class DataSetJson
{
    public ActionJson[] Actions { get; set; }
    public UnitJson[] Units { get; set; }

    #region Innere Klassen

    /// <summary>
    /// Stellt alle Parameter einer <see cref="Action"/> in Json dar
    /// </summary>
    public class ActionJson
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ActionStepJson[] ActionSteps { get; set; }

        /// <summary>
        /// Diese Klasse wird zum De/Serialisieren von <see cref="ActionStep"/> verwendet 
        /// </summary>
        public class ActionStepJson
        {
            public StepTypeEnum StepType { get; set; }
            public SelectTypeEnum SelectType { get; set; }
            public bool AllowEmptyFields { get; set; }
            public Faction TargetFaction { get; set; }
            public Enums.Range TargetRange { get; set; }
            public AttackTypeEnum AttackType { get; set; }
            public int AttackPower { get; set; }
            public int AttackAccuracy { get; set; }

            public enum StepTypeEnum { None, Select, SwapPosition, Attack }
            public enum SelectTypeEnum { None, Manual, Auto }
            public enum AttackTypeEnum { None, Physical, Special }
        }
    }

    /// <summary>
    /// Represents the json structure for a <see cref="Unit"/>
    /// </summary>
    public class UnitJson
    {
        public string Codename { get; set; }
        public string Icon { get; set; }
        public UnitLevelJson Level1 { get; set; }
        public UnitLevelJson Level2 { get; set; }
        public UnitLevelJson Level3 { get; set; }
        public UnitLevelJson Level4 { get; set; }

        /// <summary>
        /// Represents the level of a unit
        /// </summary>
        public class UnitLevelJson
        {
            public string Name { get; set; }
            public string Sprite { get; set; }
            public UnitLevelJson_Stats Stats { get; set; }
            public string[] Actions { get; set; } // Array with the IDs of the Actions this Unit knows

            public class UnitLevelJson_Stats
            {
                public int HP { get; set; }
                public int Strength { get; set; }
                public int Toughness { get; set; }
                public int Precision { get; set; }
                public int Agility { get; set; }
                public int Power { get; set; }
                public int Defense { get; set; }
                public int Aura { get; set; }
                public int Willpower { get; set; }
            }
        }
    }

    #endregion
}