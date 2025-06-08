using MA_Core.Data;
using MA_Core.Data.ValueObjects;

namespace MA_Test.Tests;

public class DataSetTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ConstructorTest()
    {
        var dataSetJson = new DataSetJson
        {
            Actions =
            [
                new DataSetJson.ActionJson()
                {
                    ID = "000",
                    Name = "Test",
                    Description = "Test",
                    ActionSteps =
                    [
                        new DataSetJson.ActionJson.ActionStepJson()
                        {
                            StepType = DataSetJson.ActionJson.ActionStepJson.StepTypeEnum.Select,
                            SelectType = DataSetJson.ActionJson.ActionStepJson.SelectTypeEnum.Self
                        }
                    ]
                },
                new DataSetJson.ActionJson()
                {
                    ID = "001",
                    Name = "Test2",
                    Description = "Test2",
                    ActionSteps =
                    [
                        new DataSetJson.ActionJson.ActionStepJson()
                        {
                            StepType = DataSetJson.ActionJson.ActionStepJson.StepTypeEnum.Select,
                            SelectType = DataSetJson.ActionJson.ActionStepJson.SelectTypeEnum.Manual,
                            SelectCount = 1,
                            AllowEmptyFields = true
                        },
                        new DataSetJson.ActionJson.ActionStepJson()
                        {
                            StepType = DataSetJson.ActionJson.ActionStepJson.StepTypeEnum.SwapPosition
                        }
                    ]
                }
            ],
            Units =
            [
                new DataSetJson.UnitJson()
                {
                    Codename = UnitCodeName.From("Test"),
                    Icon = "TestIcon",
                    Level1 = new DataSetJson.UnitJson.UnitLevelJson()
                    {
                        Name = "TestLevel1",
                        Sprite = "TestSprite1",
                        Actions = ["000"],
                        Stats = new DataSetJson.UnitJson.UnitLevelJson.UnitLevelJson_Stats()
                        {
                            HP = 100,
                            Strength = 10,
                            Defense = 10,
                            Agility = 10,
                            Toughness = 10,
                            Aura = 10,
                            Precision = 10,
                            Power = 10,
                            Willpower = 10
                        }
                    },
                    Level2 = new DataSetJson.UnitJson.UnitLevelJson()
                    {
                        Name = "TestLevel2",
                        Sprite = "TestSprite2",
                        Actions = ["000"],
                        Stats = new DataSetJson.UnitJson.UnitLevelJson.UnitLevelJson_Stats()
                        {
                            HP = 200,
                            Strength = 20,
                            Defense = 20,
                            Agility = 20,
                            Toughness = 20,
                            Aura = 20,
                            Precision = 20,
                            Power = 20,
                            Willpower = 20
                        }
                    },
                    Level3 = new DataSetJson.UnitJson.UnitLevelJson()
                    {
                        Name = "TestLevel3",
                        Sprite = "TestSprite3",
                        Actions = ["000", "001"],
                        Stats = new DataSetJson.UnitJson.UnitLevelJson.UnitLevelJson_Stats()
                        {
                            HP = 300,
                            Strength = 30,
                            Defense = 30,
                            Agility = 30,
                            Toughness = 30,
                            Aura = 30,
                            Precision = 30,
                            Power = 30,
                            Willpower = 30
                        }
                    },
                    Level4 = new DataSetJson.UnitJson.UnitLevelJson()
                    {
                        Name = "TestLevel4",
                        Sprite = "TestSprite4",
                        Actions = ["000", "001"],
                        Stats = new DataSetJson.UnitJson.UnitLevelJson.UnitLevelJson_Stats()
                        {
                            HP = 400,
                            Strength = 40,
                            Defense = 40,
                            Agility = 40,
                            Toughness = 40,
                            Aura = 40,
                            Precision = 40,
                            Power = 40,
                            Willpower = 40
                        }
                    }
                }
            ]
        };

        var metadataJson = new DataSetMetadataJson
        {
            Version = Versions.DataSetVersion
        };

        var dataset = DataSet.Test_Factory(dataSetJson, metadataJson);
        
        Assert.Multiple(() =>
        {
            Assert.That(dataset.Actions, Has.Count.EqualTo(2));
            Assert.That(dataset.Units, Has.Count.EqualTo(1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(dataset.Actions[0].ID, Is.EqualTo("000"));
            Assert.That(dataset.Actions[0].Name, Is.EqualTo("Test"));
            Assert.That(dataset.Actions[0].Description, Is.EqualTo("Test"));
            Assert.That(dataset.Actions[0].Steps, Has.Length.EqualTo(1));
            
            Assert.That(dataset.Actions[1].ID, Is.EqualTo("001"));
            Assert.That(dataset.Actions[1].Name, Is.EqualTo("Test2"));
            Assert.That(dataset.Actions[1].Description, Is.EqualTo("Test2"));
            Assert.That(dataset.Actions[1].Steps, Has.Length.EqualTo(2));
            
            Assert.That(dataset.Units[0].Codename, Is.EqualTo("Test"));
            Assert.That(dataset.Units[0].IconPath, Is.EqualTo("TestIcon"));
            
            Assert.That(dataset.Units[0].Level1.Name, Is.EqualTo("TestLevel1"));
            Assert.That(dataset.Units[0].Level1.SpritePath, Is.EqualTo("TestSprite1"));
            Assert.That(dataset.Units[0].Level1.Actions, Has.Length.EqualTo(1));
            Assert.That(dataset.Units[0].Level1.Actions[0].ID, Is.EqualTo("000"));
            Assert.That(ReferenceEquals(dataset.Units[0].Level1.Actions[0], dataset.Actions[0]), Is.True);
            Assert.That(dataset.Units[0].Level1.HP, Is.EqualTo(100));
            Assert.That(dataset.Units[0].Level1.Strength, Is.EqualTo(10));
            Assert.That(dataset.Units[0].Level1.Toughness, Is.EqualTo(10));
            Assert.That(dataset.Units[0].Level1.Precision, Is.EqualTo(10));
            Assert.That(dataset.Units[0].Level1.Agility, Is.EqualTo(10));
            Assert.That(dataset.Units[0].Level1.Power, Is.EqualTo(10));
            Assert.That(dataset.Units[0].Level1.Defense, Is.EqualTo(10));
            Assert.That(dataset.Units[0].Level1.Aura, Is.EqualTo(10));
            Assert.That(dataset.Units[0].Level1.Willpower, Is.EqualTo(10));
            
            Assert.That(dataset.Units[0].Level2.Name, Is.EqualTo("TestLevel2"));
            Assert.That(dataset.Units[0].Level2.SpritePath, Is.EqualTo("TestSprite2"));
            Assert.That(dataset.Units[0].Level2.Actions, Has.Length.EqualTo(1));
            Assert.That(dataset.Units[0].Level2.Actions[0].ID, Is.EqualTo("000"));
            Assert.That(ReferenceEquals(dataset.Units[0].Level2.Actions[0], dataset.Actions[0]), Is.True);
            Assert.That(dataset.Units[0].Level2.HP, Is.EqualTo(200));
            Assert.That(dataset.Units[0].Level2.Strength, Is.EqualTo(20));
            Assert.That(dataset.Units[0].Level2.Toughness, Is.EqualTo(20));
            Assert.That(dataset.Units[0].Level2.Precision, Is.EqualTo(20));
            Assert.That(dataset.Units[0].Level2.Agility, Is.EqualTo(20));
            Assert.That(dataset.Units[0].Level2.Power, Is.EqualTo(20));
            Assert.That(dataset.Units[0].Level2.Defense, Is.EqualTo(20));
            Assert.That(dataset.Units[0].Level2.Aura, Is.EqualTo(20));
            Assert.That(dataset.Units[0].Level2.Willpower, Is.EqualTo(20));
            
            Assert.That(dataset.Units[0].Level3.Name, Is.EqualTo("TestLevel3"));
            Assert.That(dataset.Units[0].Level3.SpritePath, Is.EqualTo("TestSprite3"));
            Assert.That(dataset.Units[0].Level3.Actions, Has.Length.EqualTo(2));
            Assert.That(dataset.Units[0].Level3.Actions[0].ID, Is.EqualTo("000"));
            Assert.That(dataset.Units[0].Level3.Actions[1].ID, Is.EqualTo("001"));
            Assert.That(ReferenceEquals(dataset.Units[0].Level3.Actions[0], dataset.Actions[0]), Is.True);
            Assert.That(ReferenceEquals(dataset.Units[0].Level3.Actions[1], dataset.Actions[1]), Is.True);
            Assert.That(dataset.Units[0].Level3.HP, Is.EqualTo(300));
            Assert.That(dataset.Units[0].Level3.Strength, Is.EqualTo(30));
            Assert.That(dataset.Units[0].Level3.Toughness, Is.EqualTo(30));
            Assert.That(dataset.Units[0].Level3.Precision, Is.EqualTo(30));
            Assert.That(dataset.Units[0].Level3.Agility, Is.EqualTo(30));
            Assert.That(dataset.Units[0].Level3.Power, Is.EqualTo(30));
            Assert.That(dataset.Units[0].Level3.Defense, Is.EqualTo(30));
            Assert.That(dataset.Units[0].Level3.Aura, Is.EqualTo(30));
            Assert.That(dataset.Units[0].Level3.Willpower, Is.EqualTo(30));
            
            Assert.That(dataset.Units[0].Level4.Name, Is.EqualTo("TestLevel4"));
            Assert.That(dataset.Units[0].Level4.SpritePath, Is.EqualTo("TestSprite4"));
            Assert.That(dataset.Units[0].Level4.Actions, Has.Length.EqualTo(2));
            Assert.That(dataset.Units[0].Level4.Actions[0].ID, Is.EqualTo("000"));
            Assert.That(dataset.Units[0].Level4.Actions[1].ID, Is.EqualTo("001"));
            Assert.That(ReferenceEquals(dataset.Units[0].Level4.Actions[0], dataset.Actions[0]), Is.True);
            Assert.That(ReferenceEquals(dataset.Units[0].Level4.Actions[1], dataset.Actions[1]), Is.True);
            Assert.That(dataset.Units[0].Level4.HP, Is.EqualTo(400));
            Assert.That(dataset.Units[0].Level4.Strength, Is.EqualTo(40));
            Assert.That(dataset.Units[0].Level4.Toughness, Is.EqualTo(40));
            Assert.That(dataset.Units[0].Level4.Precision, Is.EqualTo(40));
            Assert.That(dataset.Units[0].Level4.Agility, Is.EqualTo(40));
            Assert.That(dataset.Units[0].Level4.Power, Is.EqualTo(40));
            Assert.That(dataset.Units[0].Level4.Defense, Is.EqualTo(40));
            Assert.That(dataset.Units[0].Level4.Aura, Is.EqualTo(40));
            Assert.That(dataset.Units[0].Level4.Willpower, Is.EqualTo(40));
        });
    }
}