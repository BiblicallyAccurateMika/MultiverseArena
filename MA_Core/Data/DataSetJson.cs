using MA_Core.Data.Enums;

namespace MA_Core.Data;

/// <summary>
/// Diese Klasse wird für das Serialisieren und Deserialisieren von Datasets verwendet
/// </summary>
public class DataSetJson
{
    public ActionJson[] Actions { get; init; }
    public UnitJson[] Units { get; init; }

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

        public Action AsAction()
        {
            return new Action
            {
                ID = ID,
                Name = Name,
                Description = Description,
                Steps = ActionSteps.Select(x => x.AsActionStep()).ToArray()
            };
        }

        /// <summary>
        /// Diese Klasse wird zum De/Serialisieren von <see cref="ActionStep"/> verwendet 
        /// </summary>
        public class ActionStepJson
        {
            public StepTypeEnum StepType { get; set; }
            public SelectTypeEnum SelectType { get; set; }
            public bool AllowEmptyFields { get; set; }
            public bool AllowSelf { get; set; }
            public int SelectCount { get; set; }
            public Faction TargetFaction { get; set; }
            public Enums.Range TargetRange { get; set; }
            public AttackTypeEnum AttackType { get; set; }
            public int AttackPower { get; set; }
            public int AttackAccuracy { get; set; }

            public enum StepTypeEnum { None, Select, SwapPosition, Attack }
            public enum SelectTypeEnum { None, Manual, Auto }
            public enum AttackTypeEnum { None, Physical, Special }

            public ActionStep AsActionStep()
            {
                switch (StepType)
                {
                    //todo: Fill in missing types
                    case StepTypeEnum.Select:
                        if (SelectType == SelectTypeEnum.Manual)
                        {
                            //todo: Fill in missing fields
                            return new ActionStep.Select.Arbitrary.Manual
                            {
                                Faction = TargetFaction,
                                SelectionCount = SelectCount > 0 ? SelectCount : 1
                            };
                        }
                        else
                        {
                            return new ActionStep.Select.Arbitrary.Automatic
                            {
                                Faction = TargetFaction,
                                Range = TargetRange,
                                AllowSelf = AllowSelf
                            };
                        }
                    case StepTypeEnum.SwapPosition: return new ActionStep.SwapPosition();
                    case StepTypeEnum.Attack:
                        if (AttackType == AttackTypeEnum.Physical)
                        {
                            return new ActionStep.PhysicalAttack
                            {
                                Accuracy = AttackAccuracy,
                                Power = AttackPower
                            };
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    default: throw new NotImplementedException();
                }
            }
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

        public Unit AsUnit(ICollection<Action> actions)
        {
            return new Unit()
            {
                Codename = Codename,
                IconPath = Icon,
                Level1 = Level1.AsLevel(actions),
                Level2 = Level2.AsLevel(actions),
                Level3 = Level3.AsLevel(actions),
                Level4 = Level4.AsLevel(actions),
            };
        }

        /// <summary>
        /// Represents the level of a unit
        /// </summary>
        public class UnitLevelJson
        {
            public string Name { get; set; }
            public string Sprite { get; set; }
            public UnitLevelJson_Stats Stats { get; set; }
            public string[] Actions { get; set; } // Array with the IDs of the Actions this Unit knows

            public Unit.Level AsLevel(ICollection<Action> actions)
            {
                if (Actions.Any(x => !actions.Any(y => y.ID.Equals(x))))
                {
                   throw new Exception("Actions not found"); 
                }
                
                return new Unit.Level
                {
                    Name = Name,
                    SpritePath = Sprite,
                    Actions = actions.Where(x => Actions.Contains(x.ID)).ToArray(),
                    HP = Stats.HP,
                    Strength = Stats.Strength,
                    Toughness = Stats.Toughness,
                    Precision = Stats.Precision,
                    Agility = Stats.Agility,
                    Power = Stats.Power,
                    Defense = Stats.Defense,
                    Aura = Stats.Aura,
                    Willpower = Stats.Willpower,
                };
            }

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