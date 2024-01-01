using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.UI.Gumps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;

namespace ClassicUO.Game.Managers
{
    [JsonSerializable(typeof(ToolTipOverrideData))]
    internal class ToolTipOverrideData
    {
        private static readonly Dictionary<string, string> StatNameTransforms = new Dictionary<string, string>()
        {
            { "Hit Chance Increase", "HCI" },
            { "Damage Increase", "DI" },
            { "Swing Speed Increase", "SSI" },
            { "Lower Mana Cost", "LMC" },
            { "Lower Reagent Cost", "LRC" },
            { "Strength Bonus", "+Str" },
            { "Dexterity Bonus", "+Dex" },
            { "Intelligence Bonus", "+Int" },
            { "Hit Point Increase", "+HP" },
            { "Stamina Increase", "+Stam" },
            { "Mana Increase", "+Mana" },
            { "Hit Point Regeneration", "+HP Regen" },
            { "Stamina Regeneration", "Stam Regen" },
            { "Mana Regeneration", "Mana Regen" },
            { "Defense Chance Increase", "DCI" },
            { "Spell Damage Increase", "SDI" },
            { "Faster Casting", "FC" },
            { "Faster Cast Recovery", "FCR" },
            { "Reflect Physical Damage", "RPD" },
            { "Enhance Potions", "EP" },
            { "Luck", "Luck" },
            { "Damage Eater", "DE" },
            { "Physical Resist", "Phy Res" },
            { "Fire Resist", "Fire Res" },
            { "Cold Resist", "Cold Res" },
            { "Poison Resist", "Poison Res" },
            { "Energy Resist", "Energy Res" },
        };

        private static readonly HashSet<string> Skills = new HashSet<string>
        (
            new List<string>
            {
                "Archery",
                "Chivalry",
                "Fencing",
                "Focus",
                "Mace Fighting",
                "Parrying",
                "Swordsmanship",
                "Tactics",
                "Wrestling",
                "Bushido",
                "Throwing",
                "Healing",
                "Veterinary",
                "Magical",
                "Alchemy",
                "Glassblowing",
                "Evaluating Intelligence",
                "Inscription",
                "Magery",
                "Meditation",
                "Necromancy",
                "Resisting Spells",
                "Spellweaving",
                "Spirit Speak",
                "Mysticism",
                "Bardic",
                "Discordance",
                "Musicianship",
                "Peacemaking",
                "Provocation",
                "Rogue",
                "Begging",
                "Detecting Hidden",
                "Hiding",
                "Lockpicking",
                "Poisoning",
                "Remove Trap",
                "Snooping",
                "Stealing",
                "Stealth",
                "Ninjitsu",
                "Creatures & Sensing",
                "Anatomy",
                "Animal Lore",
                "Animal Taming",
                "Camping",
                "Forensic Evaluation",
                "Herding",
                "Taste Identification",
                "Tracking",
                "Crafting",
                "Arms Lore",
                "Blacksmithy",
                "Bowcraft & Fletching",
                "Carpentry",
                "Masonry",
                "Cooking",
                "Item Identification",
                "Cartography",
                "Tailoring",
                "Tinkering",
                "Basket Weaving",
                "Imbuing",
                "Resource Gathering",
                "Fishing",
                "Mining",
                "Lumberjacking"
            }
        );

        private static List<string> SingleStats = new List<string>()
        {
            "Antique",
            "Balanced",
            "Bane",
            "Berserk",
            "Brittle",
            "Cursed",
            "Focus",
            "Massive",
            "Prized",
            "Sparks",
            "Swarm",
            // "Mage Armor",
            // "Unwieldy",
            // "Battle Lust",
            // "Blood Drinker",
            // "Bone Breaker",
            // "Assassin Honed",
            // "Reactive Paralyze",
            // "Spell Channeling",
            // "Spell Focusing",
        };

        private static double? GetPlayerDataByKey(string key)
        {
            var value = key.Replace(" ", "");

            switch (value)
            {
                case "ColdResist": return World.Player.ColdResistance;
                case "DamageIncrease": return World.Player.DamageIncrease;
                case "DamageMax": return World.Player.DamageMax;
                case "DamageMin": return World.Player.DamageMin;
                case "DeathScreenTimer": return World.Player.DeathScreenTimer;
                case "DefenseChanceIncrease": return World.Player.DefenseChanceIncrease;
                case "Dexterity": return World.Player.Dexterity;
                case "DexterityIncrease": return World.Player.DexterityIncrease;
                case "EnergyResist": return World.Player.EnergyResistance;
                case "EnhancePotions": return World.Player.EnhancePotions;
                case "FasterCasting": return World.Player.FasterCasting;
                case "FasterCastRecovery": return World.Player.FasterCastRecovery;
                case "FireResist": return World.Player.FireResistance;
                case "Followers": return World.Player.Followers;
                case "FollowersMax": return World.Player.FollowersMax;
                case "Gold": return World.Player.Gold;
                case "HitChanceIncrease": return World.Player.HitChanceIncrease;
                case "HitPointsIncrease": return World.Player.HitPointsIncrease;
                case "HitPointsRegeneration": return World.Player.HitPointsRegeneration;
                case "Intelligence": return World.Player.Intelligence;
                case "IntelligenceIncrease": return World.Player.IntelligenceIncrease;
                case "LowerManaCost": return World.Player.LowerManaCost;
                case "LowerReagentCost": return World.Player.LowerReagentCost;
                case "Luck": return World.Player.Luck;
                case "ManaIncrease": return World.Player.ManaIncrease;
                case "ManaRegeneration": return World.Player.ManaRegeneration;
                case "MaxColdResist": return World.Player.MaxColdResistence;
                case "MaxDefenseChanceIncrease": return World.Player.MaxDefenseChanceIncrease;
                case "MaxEnergyResist": return World.Player.MaxEnergyResistence;
                case "MaxFireResist": return World.Player.MaxFireResistence;
                case "MaxHitPointsIncrease": return World.Player.MaxHitPointsIncrease;
                case "MaxManaIncrease": return World.Player.MaxManaIncrease;
                case "MaxPhysicalResist": return World.Player.MaxPhysicResistence;
                case "MaxPoisonResist": return World.Player.MaxPoisonResistence;
                case "MaxStaminaIncrease": return World.Player.MaxStaminaIncrease;
                case "PhysicalResist": return World.Player.PhysicalResistance;
                case "PoisonResist": return World.Player.PoisonResistance;
                case "ReflectPhysicalDamage": return World.Player.ReflectPhysicalDamage;
                case "SpellDamageIncrease": return World.Player.SpellDamageIncrease;
                case "StaminaIncrease": return World.Player.StaminaIncrease;
                case "StaminaRegeneration": return World.Player.StaminaRegeneration;
                case "StatsCap": return World.Player.StatsCap;
                case "Strength": return World.Player.Strength;
                case "StrengthIncrease": return World.Player.StrengthIncrease;
                case "SwingSpeedIncrease": return World.Player.SwingSpeedIncrease;
                case "TithingPoints": return World.Player.TithingPoints;
                case "Weight": return World.Player.Weight;
                case "WeightMax": return World.Player.WeightMax;
            }

            return null;
        }

        public ToolTipOverrideData()
        {
        }

        public ToolTipOverrideData(int index, string searchText, string formattedText, int min1, int max1, int min2, int max2, byte layer)
        {
            Index = index;
            SearchText = searchText;
            FormattedText = formattedText;
            Min1 = min1;
            Max1 = max1;
            Min2 = min2;
            Max2 = max2;
            ItemLayer = (TooltipLayers)layer;
        }

        private string searchText, formattedText;

        public int Index { get; }

        public string SearchText
        {
            get { return searchText.Replace(@"\u002B", @"+"); }
            set { searchText = value; }
        }

        public string FormattedText
        {
            get { return formattedText.Replace(@"\u002B", @"+"); }
            set { formattedText = value; }
        }

        public int Min1 { get; set; }
        public int Max1 { get; set; }
        public int Min2 { get; set; }
        public int Max2 { get; set; }
        public TooltipLayers ItemLayer { get; set; }

        public bool IsNew { get; set; } = false;

        public static ToolTipOverrideData Get(int index)
        {
            bool isNew = false;

            if (ProfileManager.CurrentProfile != null)
            {
                string searchText = "Weapon Damage", formattedText = "DMG /c[orange]{1} /cd- /c[red]{2}";
                int min1 = -1, max1 = 99, min2 = -1, max2 = 99;
                byte layer = (byte)TooltipLayers.Any;

                if (ProfileManager.CurrentProfile.ToolTipOverride_SearchText.Count > index)
                    searchText = ProfileManager.CurrentProfile.ToolTipOverride_SearchText[index];
                else
                    isNew = true;

                if (ProfileManager.CurrentProfile.ToolTipOverride_NewFormat.Count > index)
                    formattedText = ProfileManager.CurrentProfile.ToolTipOverride_NewFormat[index];
                else
                    isNew = true;

                if (ProfileManager.CurrentProfile.ToolTipOverride_MinVal1.Count > index)
                    min1 = ProfileManager.CurrentProfile.ToolTipOverride_MinVal1[index];
                else
                    isNew = true;

                if (ProfileManager.CurrentProfile.ToolTipOverride_MinVal2.Count > index)
                    min2 = ProfileManager.CurrentProfile.ToolTipOverride_MinVal2[index];
                else
                    isNew = true;

                if (ProfileManager.CurrentProfile.ToolTipOverride_MaxVal1.Count > index)
                    max1 = ProfileManager.CurrentProfile.ToolTipOverride_MaxVal1[index];
                else
                    isNew = true;

                if (ProfileManager.CurrentProfile.ToolTipOverride_MaxVal2.Count > index)
                    max2 = ProfileManager.CurrentProfile.ToolTipOverride_MaxVal2[index];
                else
                    isNew = true;

                if (ProfileManager.CurrentProfile.ToolTipOverride_Layer.Count > index)
                    layer = ProfileManager.CurrentProfile.ToolTipOverride_Layer[index];
                else
                    isNew = true;

                ToolTipOverrideData data = new ToolTipOverrideData(index, searchText, formattedText, min1, max1, min2, max2, layer);

                if (isNew)
                {
                    data.IsNew = true;
                    data.Save();
                }

                return data;
            }

            return null;
        }

        public void Save()
        {
            if (ProfileManager.CurrentProfile.ToolTipOverride_SearchText.Count > Index)
                ProfileManager.CurrentProfile.ToolTipOverride_SearchText[Index] = SearchText;
            else
                ProfileManager.CurrentProfile.ToolTipOverride_SearchText.Add(SearchText);

            if (ProfileManager.CurrentProfile.ToolTipOverride_NewFormat.Count > Index)
                ProfileManager.CurrentProfile.ToolTipOverride_NewFormat[Index] = FormattedText;
            else
                ProfileManager.CurrentProfile.ToolTipOverride_NewFormat.Add(FormattedText);

            if (ProfileManager.CurrentProfile.ToolTipOverride_MinVal1.Count > Index)
                ProfileManager.CurrentProfile.ToolTipOverride_MinVal1[Index] = Min1;
            else
                ProfileManager.CurrentProfile.ToolTipOverride_MinVal1.Add(Min1);

            if (ProfileManager.CurrentProfile.ToolTipOverride_MinVal2.Count > Index)
                ProfileManager.CurrentProfile.ToolTipOverride_MinVal2[Index] = Min2;
            else
                ProfileManager.CurrentProfile.ToolTipOverride_MinVal2.Add(Min2);

            if (ProfileManager.CurrentProfile.ToolTipOverride_MaxVal1.Count > Index)
                ProfileManager.CurrentProfile.ToolTipOverride_MaxVal1[Index] = Max1;
            else
                ProfileManager.CurrentProfile.ToolTipOverride_MaxVal1.Add(Max1);

            if (ProfileManager.CurrentProfile.ToolTipOverride_MaxVal2.Count > Index)
                ProfileManager.CurrentProfile.ToolTipOverride_MaxVal2[Index] = Max2;
            else
                ProfileManager.CurrentProfile.ToolTipOverride_MaxVal2.Add(Max2);

            if (ProfileManager.CurrentProfile.ToolTipOverride_Layer.Count > Index)
                ProfileManager.CurrentProfile.ToolTipOverride_Layer[Index] = (byte)ItemLayer;
            else
                ProfileManager.CurrentProfile.ToolTipOverride_Layer.Add((byte)ItemLayer);
        }

        public void Delete()
        {
            ProfileManager.CurrentProfile.ToolTipOverride_SearchText.RemoveAt(Index);
            ProfileManager.CurrentProfile.ToolTipOverride_NewFormat.RemoveAt(Index);
            ProfileManager.CurrentProfile.ToolTipOverride_MinVal1.RemoveAt(Index);
            ProfileManager.CurrentProfile.ToolTipOverride_MinVal2.RemoveAt(Index);
            ProfileManager.CurrentProfile.ToolTipOverride_MaxVal1.RemoveAt(Index);
            ProfileManager.CurrentProfile.ToolTipOverride_MaxVal2.RemoveAt(Index);
            ProfileManager.CurrentProfile.ToolTipOverride_Layer.RemoveAt(Index);
        }

        public static ToolTipOverrideData[] GetAllToolTipOverrides()
        {
            if (ProfileManager.CurrentProfile == null)
                return null;

            ToolTipOverrideData[] result = new ToolTipOverrideData[ProfileManager.CurrentProfile.ToolTipOverride_SearchText.Count];

            for (int i = 0; i < ProfileManager.CurrentProfile.ToolTipOverride_SearchText.Count; i++)
            {
                result[i] = Get(i);
            }

            return result;
        }

        public static void ExportOverrideSettings()
        {
            ToolTipOverrideData[] allData = GetAllToolTipOverrides();

            if (!CUOEnviroment.IsUnix)
            {
                Thread t = new Thread
                (
                    () =>
                    {
                        System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                        saveFileDialog1.Filter = "Json|*.json";
                        saveFileDialog1.Title = "Save tooltip override settings";
                        saveFileDialog1.ShowDialog();

                        string result = JsonSerializer.Serialize(allData);

                        // If the file name is not an empty string open it for saving.
                        if (saveFileDialog1.FileName != "")
                        {
                            System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();

                            // NOTE that the FilterIndex property is one-based.
                            switch (saveFileDialog1.FilterIndex)
                            {
                                default:
                                    byte[] data = Encoding.UTF8.GetBytes(result);
                                    fs.Write(data, 0, data.Length);

                                    break;
                            }

                            fs.Close();
                        }
                    }
                );

                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }

        public static void ImportOverrideSettings()
        {
            if (!CUOEnviroment.IsUnix)
            {
                Thread t = new Thread
                (
                    () =>
                    {
                        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
                        openFileDialog.Filter = "Json|*.json";
                        openFileDialog.Title = "Import tooltip override settings";
                        openFileDialog.ShowDialog();

                        // If the file name is not an empty string open it for saving.
                        if (openFileDialog.FileName != "")
                        {
                            // NOTE that the FilterIndex property is one-based.
                            switch (openFileDialog.FilterIndex)
                            {
                                default:
                                    try
                                    {
                                        string result = File.ReadAllText(openFileDialog.FileName);

                                        ToolTipOverrideData[] imported = JsonSerializer.Deserialize<ToolTipOverrideData[]>(result);

                                        foreach (ToolTipOverrideData importedData in imported)
                                            //GameActions.Print(importedData.searchText);
                                            new ToolTipOverrideData
                                            (
                                                ProfileManager.CurrentProfile.ToolTipOverride_SearchText.Count, importedData.searchText, importedData.FormattedText,
                                                importedData.Min1, importedData.Max1, importedData.Min2, importedData.Max2, (byte)importedData.ItemLayer
                                            ).Save();

                                        ToolTipOverideMenu.Reopen = true;
                                    }
                                    catch (System.Exception e)
                                    {
                                        GameActions.Print("It looks like there was an error trying to import your override settings.", 32);
                                    }

                                    break;
                            }
                        }
                    }
                );

                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
        }

        private static string GetLayerCategory(Item item)
        {
            switch ((Layer)item.ItemData.Layer)
            {
                case Layer.TwoHanded when item.ItemData.Name.ToLowerInvariant().Contains("bow") || item.ItemData.Name.ToLowerInvariant().Contains("yumi"): return "Ranged";

                case Layer.TwoHanded:
                case Layer.OneHanded:
                    var itemName = item.ItemData.Name.ToLowerInvariant();

                    if (itemName.Contains("shield") || itemName.Contains("buckler"))
                        return "Shield";

                    // TODO: Support spellbooks
                    return "Melee";

                case Layer.Ring:
                case Layer.Necklace:
                case Layer.Bracelet:
                case Layer.Earrings: return "Jewelry";

                case Layer.Talisman: return "Talisman";

                default: return "Armor";
            }
        }

        private static (int, int)? GetMinMaxFromItemStat(Item item, string statName)
        {
            var itemType = GetLayerCategory(item);
            
            if (statName.EndsWith("Resist"))
            {
                return itemType switch
                {
                    "Armor" => (1, 30),
                    "Shield" => (1, 15),
                    _ => (1, 20)
                };
            }
            
            if (Skills.Contains(statName.Replace("+", "").Trim()))
            {
                return itemType switch
                {
                    "Jewelry" => (1, 20),
                    _ => (1, 12)
                };
            }

            // TODO: Read ranges from a json object that can be customized by the user.
            switch (statName)
            {
                case "Casting Focus": return (1, 3);
                case "Chaos Damage": return (10, 100);
                case "Craft Bonus": return (1, 30);
                case "Craft Exceptional Bonus": return (1, 30);
                case "Damage": return (10, 100);
                case "Damage Eater": return (3, 15);
                case "Damage Increase":
                    return itemType switch
                    {
                        "Jewelry" => (1, 35),
                        "Shield" => (5, 35),
                        _ => (1, 70)
                    };

                case "Defense Chance Increase":
                    return itemType switch
                    {
                        "Ranged" => (1, 35),
                        "Armor" => (1, 5),
                        _ => (1, 20)
                    };
                case "Dexterity Bonus":
                    return itemType switch
                    {
                        "Jewelry" => (1, 10),
                        _ => (1, 5)
                    };
                case "Durability Bonus": return (1, 150);
                case "Enhance Potions":
                    return itemType switch
                    {
                        "Jewelry" => (5, 35),
                        "Armor" => (1, 3),
                        _ => (5, 15)
                    };
                case "Faster Cast Recovery": return (1, 4);
                case "Faster Casting": return (1, 1);
                case "Hit Area Damage": return (2, 70);
                case "Hit Chance Increase": 
                    switch (itemType)
                    {
                        case "Melee":
                        case "Jewelry": return (1, 35);
                        case "Ranged": return (1, 5);
                        default: return (1, 5);
                    }
                case "Hit Curse": return (2, 50);
                case "Hit Dispel": return (2, 70);
                case "Hit Fatigue": return (2, 70);
                case "Hit Fireball": return (2, 70);
                case "Hit Harm": return (2, 70);
                case "Hit Energy Area": return (2, 70);
                case "Hit Cold Area": return (2, 70);
                case "Hit Life Leech": return (2, 100);
                case "Hit Lightning": return (2, 70);
                case "Hit Lower Attack": return (2, 70);
                case "Hit Lower Defense": return (2, 70);
                case "Hit Magic Arrow": return (2, 70);
                case "Hit Mana Drain": return (2, 70);
                case "Hit Mana Leech": return (2, 100);
                case "Hit Point Increase": return (1, 7);
                case "Hit Point Regeneration": 
                    switch (itemType)
                    {
                        case "Melee": 
                        case "Ranged": return (1, 9);
                        default: return (1, 4);
                    }
                case "Hit Stamina Leech": return (2, 100);
                case "Intelligence Bonus":
                    return itemType switch
                    {
                        "Jewelry" => (1, 10),
                        _ => (1, 5)
                    };
                case "Lower Mana Cost": 
                    switch (itemType)
                    {
                        case "Armor":
                        case "Jewelry": return (1, 10);
                        default: return (1, 5);
                    }
                case "Lower Reagent Cost": return (1, 25);
                // case "Lower Requirements": return (10, 100);
                case "Luck": return (1, 150);
                case "Mage Weapon": return (-29, -15);
                case "Mana Increase":
                    return itemType switch
                    {
                        "Armor" => (1, 10),
                        _ => (1, 5)
                    };
                case "Mana Regeneration":
                    switch (itemType)
                    {
                        case "Armor": 
                        case "Shields":
                        case "Jewelry": return (1, 4);
                        default: return (1, 9);
                    }
                case "Reflect Physical Damage": return (1, 20);
                case "Resonance": return (1, 20);
                case "Searing": return (1, 7);
                case "Self Repair":
                    return itemType switch
                    {
                        "Armor" => (1, 5),
                        _ => (1, 3)
                    };
                case "Soul Charge": return (5, 30);
                case "Spell Damage Increase":
                    return itemType switch
                    {
                        "Jewelry" => (1, 18),
                        _ => (1, 9)
                    };
                case "Splinting Weapon": return (1, 30);
                case "Stamina Increase":
                    return itemType switch
                    {
                        "Armor" => (1, 10),
                        _ => (1, 5)
                    };
                case "Stamina Regeneration":
                    switch (itemType)
                    {
                        case "Armor": 
                        case "Shields": return (1, 4);
                        default: return (1, 9);
                    }
                case "Strength Bonus":
                    return itemType switch
                    {
                        "Jewelry" => (1, 10),
                        _ => (1, 5)
                    };
                case "Swing Speed Increase":
                    switch (itemType)
                    {
                        case "Ranged":
                        case "Melee": return (5, 40);
                        default: return (1, 10);
                    };
                case "Velocity": return (2, 50);
                case "Weight Reduction": return (30, 50);
                default: return null;
            }
        }
        
        static Dictionary<int, string> colorMap = new Dictionary<int, string>
        {
            { 1, "#ff6666" }, // Light Red
            { 2, "#ff9966" }, // Light Red-Orange
            { 3, "#ffcc99" }, // Light Orange
            { 4, "#ffcc66" }, // Light Yellow-Orange
            { 5, "#ffff99" }, // Light Yellow
            { 6, "#ccff99" }, // Light Yellow-Green
            { 7, "#66cc66" }, // Light Green
            { 8, "#99ffff" }, // Light Cyan
            { 9, "#6699ff" }, // Light Blue
            { 10, "#cc99ff" } // Light Purple
        };
        
        


        static string GetColorFromValue(double value)
        {
            int rating = Math.Min(10, Math.Max(1, (int)Math.Ceiling(value / 10.0))); // Convert 0-100 scale to 1-10
            return colorMap[rating].ToLowerInvariant();
        }

        static string GetStarsFromValue(double value)
        {
            int rating =  Math.Min(10, Math.Max(1, (int)Math.Ceiling(value / 10.0))); 

            return rating switch
            {
                1 => "E",
                2 => "D",
                3 => "C",
                4 => "B",
                5 => "A",
                6 => "AA",
                7 => "AAA",
                8 => "S",
                9 => "SS",
                10 => "SSS",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static double? GetValueForSlayerType(Item item)
        {
            if (!item.ItemData.Name.Trim().EndsWith("Slayer"))
                return null;

            var topTierSlayers = new List<string>()
            {
                "Elemental",
                "Abyss",
                "Fey",
                "Reptile",
                "Arachnid",
                "Repond",
                "Undead",
                "Eodon",
                "Dragon",
                "Demon",
                "Ogre",
            };

            return topTierSlayers.Contains(item.ItemData.Name) ? 100 : 50;
        }

        private static (string, List<ItemPropertiesData.SinglePropertyData>) BuildSingleStatsRow(Dictionary<string, ItemPropertiesData.SinglePropertyData> propertyStatsLookup)
        {
            var output = new List<string>();
            
            if (propertyStatsLookup.Count == 0)
                return ("", []);
            
            if (propertyStatsLookup.ContainsKey("Insured"))
                propertyStatsLookup.Remove("Insured");
            
            if (propertyStatsLookup.ContainsKey("Blessed"))
                propertyStatsLookup.Remove("Blessed");
            
            if (propertyStatsLookup.ContainsKey("Night Sight"))
            {
                output.Add("NS");
                propertyStatsLookup.Remove("Night Sight");
            }
            
            foreach(var singleStat in SingleStats)
            {
                if (propertyStatsLookup.ContainsKey(singleStat))
                {
                    propertyStatsLookup.Remove(singleStat);
                    output.Add(singleStat);
                }
            }
            
            if (propertyStatsLookup.ContainsKey("Weight:"))
            {
                var stones = propertyStatsLookup["Weight:"].FirstValue;
                output.Add($"{stones} {(stones == 1 ? "Stone" : "Stones")}");
                propertyStatsLookup.Remove("Weight:");
            }
            
            return (string.Join(", ", output), propertyStatsLookup.Values.ToList());
        }

        public static string ProcessTooltipText(uint serial, uint compareTo = uint.MinValue)
        {
            string tooltip = "";
            ItemPropertiesData itemPropertiesData;

            if (compareTo != uint.MinValue)
                itemPropertiesData = new ItemPropertiesData(World.Items.Get(serial), World.Items.Get(compareTo));
            else
                itemPropertiesData = new ItemPropertiesData(World.Items.Get(serial));

            ToolTipOverrideData[] result = GetAllToolTipOverrides();

            if (!itemPropertiesData.HasData)
                return null;
            
            var statsTooltips = "";
            if (EventSink.PreProcessTooltip != null)
                EventSink.PreProcessTooltip(ref itemPropertiesData);

            var isInsured = itemPropertiesData.RawData.Contains("Insured");
            var isBlessed = itemPropertiesData.RawData.Contains("Blessed");
            var propertyStatsLookup = itemPropertiesData.singlePropertyData.ToDictionary(x => x.Name.Trim(), x => x);
            var (singleStatRow, singlePropertyData) = BuildSingleStatsRow(propertyStatsLookup);
            
            var valueTargets = new List<double>();
            var valueTargetsForOverrides = new double[result.Length];
            //Loop through each property
            foreach (ItemPropertiesData.SinglePropertyData property in singlePropertyData)
            {
                if (property.Name == null)
                    continue;
                
                if (property.Name.Contains("Required") || property.Name.Contains("Requirement"))
                    continue;
                
                var statRange = GetMinMaxFromItemStat(itemPropertiesData.item, property.Name.Trim());

                if (!statRange.HasValue)
                {
                    var slayerValue = GetValueForSlayerType(itemPropertiesData.item);
                    if (slayerValue.HasValue) valueTargets.Add(slayerValue.Value);
                }
                else
                {
                    valueTargets.Add(property.FirstValue / statRange.Value.Item2 * 100);
                }

                bool handled = false;

                //Loop though each override setting player created
                for (int i = 0; i < result.Length; i++)
                {
                    ToolTipOverrideData overrideData = result[i];
                    
                    if (overrideData == null)
                        continue;

                    var isInvalidLayer = overrideData.ItemLayer != TooltipLayers.Any && !CheckLayers(overrideData.ItemLayer, itemPropertiesData.item.ItemData.Layer);
                    // Updated line to support regex
                    var isNotMatch = !Regex.IsMatch(property.OriginalString, overrideData.SearchText, RegexOptions.IgnoreCase);
                    var notInRange1 = property.FirstValue != -1 && (!(property.FirstValue >= overrideData.Min1) || !(property.FirstValue <= overrideData.Max1));
                    var notInRange2 = property.SecondValue != -1 && (!(property.SecondValue >= overrideData.Min2) || !(property.SecondValue <= overrideData.Max2));

                    if (isInvalidLayer || isNotMatch || notInRange1 || notInRange2)
                    {
                        continue;
                    }

                    try
                    {
                        if (!statRange.HasValue)
                        {
                            var slayerValue = GetValueForSlayerType(itemPropertiesData.item);
                            valueTargetsForOverrides[i] = slayerValue ?? 100;
                        }
                        else
                        {
                            valueTargetsForOverrides[i] = property.FirstValue / statRange.Value.Item2 * 100;
                        }

                        var currentValue = GetPlayerDataByKey(property.Name);

                        var maxCurrentValue = GetPlayerDataByKey("Max" + property.Name);
                        var currentValues = new List<double>();

                        if (currentValue.HasValue)
                            currentValues.Add(currentValue.Value);

                        if (maxCurrentValue.HasValue)
                            currentValues.Add(maxCurrentValue.Value);
                        var originalStringWithoutColor = Regex.Replace(property.OriginalString, @"\/c\[#.{6}\]|(\/cd)", "", RegexOptions.IgnoreCase);

                        statsTooltips += string.Format
                            (
                                overrideData.FormattedText, 
                                property.Name, 
                                property.FirstValue.ToString(), 
                                property.SecondValue.ToString(), 
                                originalStringWithoutColor,
                                property.FirstDiff != 0 ? "(" + property.FirstDiff + ")" : "", 
                                property.SecondDiff != 0 ? "(" + property.SecondDiff + ")" : ""
                            ) + $"{(statRange.HasValue ? $"/c[#999999] / {statRange.Value.Item2}" : "")} ({string.Join("/", currentValues)})\n";

                        handled = true;

                        break;
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

                if (!handled)
                {
                    var field = property.Name.Replace("+", "").Trim();

                    if (field.Contains("Physical Damage") || field.Contains("Energy Damage") || field.Contains("Poison Damage") || field.Contains("Cold Damage") || field.Contains
                            ("Weapon Speed") || field.Contains("Two-handed Weapon") || field.Contains("One-handed Weapon") || field.StartsWith("Durability") || field.EndsWith
                            ("Range"))
                    {
                        statsTooltips += $"/c[#999999]{property.OriginalString.Replace("/cd", "")}\n";
                    }
                    else
                    {
                        var transformedOriginalString = property.OriginalString;

                        if (StatNameTransforms.TryGetValue(field, out var statName))
                            transformedOriginalString = transformedOriginalString.Replace(field, statName);

                        if (statRange.HasValue)
                        {
                            var currentValues = new List<double>();

                            if (Skills.Contains(field))
                            {
                                var skill = World.Player.Skills.FirstOrDefault(skill => skill.Name == field);

                                if (skill != null)
                                {
                                    var currentValue = skill.Value;
                                    var maxCurrentValue = skill.Cap;
                                    currentValues.Add(Math.Round(currentValue));
                                    currentValues.Add(maxCurrentValue);
                                }
                            }
                            else
                            {
                                var currentValue = GetPlayerDataByKey(field);
                                var maxCurrentValue = GetPlayerDataByKey("Max" + field);

                                if (currentValue.HasValue)
                                    currentValues.Add(currentValue.Value);

                                if (maxCurrentValue.HasValue)
                                    currentValues.Add(maxCurrentValue.Value);
                            }

                            var currentValueString = currentValues.Count > 0 ? $" ({string.Join("/", currentValues)})" : "";

                            var color = GetColorRamp(property.FirstValue / statRange.Value.Item2, "#FFFFFF", .1, "#FFFF00");
                            statsTooltips += $"/c[{color}]{transformedOriginalString.Replace("/cd", "")}/c[#999999] / {statRange.Value.Item2} {currentValueString}\n";
                        }
                        else
                        {
                            statsTooltips += $"{transformedOriginalString}\n";
                        }
                    }
                }
            }

            var stars = "";
            if (isBlessed)
            {
                stars = " **";
            }
            else if (isInsured)
            {
                stars = " *";
            }
            
            tooltip += ProfileManager.CurrentProfile == null ?
                $"/c[yellow]{itemPropertiesData.Name}\n" :
                string.Format(ProfileManager.CurrentProfile.TooltipHeaderFormat + stars + "\n", itemPropertiesData.Name);
            
            var itemRatings = new List<string>();

            if (valueTargets.Count > 0)
            {
                var itemValue = valueTargets.Count > 0 ? valueTargets.Average(): 0;
                string color = GetColorFromValue(itemValue);
                string rating = GetStarsFromValue(itemValue);
                itemRatings.Add($"/c[{color}]{rating}");
            }
            
            if (valueTargetsForOverrides.Length > 0 && valueTargets.Count > 0)
            {
                var itemValue = valueTargetsForOverrides.Average();
                string color = GetColorFromValue(itemValue);
                string rating = GetStarsFromValue(itemValue);
                itemRatings.Add($"/c[{color}]({rating})");
            }

            if (itemRatings.Count > 0)
                tooltip += string.Join(" ", itemRatings) + "\n";

            tooltip += singleStatRow + "\n";
            tooltip += statsTooltips;

            if (EventSink.PostProcessTooltip != null)
            {
                EventSink.PostProcessTooltip(ref tooltip);
            }

            return tooltip;
        }

        private static object GetColorRamp(double value, string destinationColor, double maxValueBuffer, string maxColor)
        {
            if (value < .25)
                return "#FFFFFF";
            if (value > 1 - maxValueBuffer)
                return maxColor;
            
            var start = ColorTranslator.FromHtml("#FFFFFF");
            var destination = ColorTranslator.FromHtml(destinationColor);

            var rDistance = destination.R - (start.R);
            var gDistance = destination.G - (start.G);
            var bDistance = destination.B - (start.B);
            var red = Math.Max(0, Math.Min(255, (int)(start.R + rDistance * value)));
            var green = Math.Max(0, Math.Min(255, (int)(start.G + gDistance * value)));
            var blue = Math.Max(0, Math.Min(255, (int)(start.B + bDistance * value)));
            
            var result = ColorTranslator.ToHtml(Color.FromArgb(red, green, blue));
            return result;
        }

        public static string ProcessTooltipText(string text)
        {
            string tooltip = "";

            ItemPropertiesData itemPropertiesData = new ItemPropertiesData(text);

            ToolTipOverrideData[] result = GetAllToolTipOverrides();

            if (itemPropertiesData.HasData && result != null && result.Length > 0)
            {
                tooltip += ProfileManager.CurrentProfile == null ?
                    $"/c[yellow]{itemPropertiesData.Name}\n" :
                    string.Format(ProfileManager.CurrentProfile.TooltipHeaderFormat + "\n", itemPropertiesData.Name);

                //Loop through each property
                foreach (ItemPropertiesData.SinglePropertyData property in itemPropertiesData.singlePropertyData)
                {
                    bool handled = false;

                    //Loop though each override setting player created
                    foreach (ToolTipOverrideData overrideData in result)
                    {
                        if (overrideData != null)
                            if (overrideData.ItemLayer == TooltipLayers.Any)
                            {
                                if (property.OriginalString.ToLower().Contains(overrideData.SearchText.ToLower()))
                                    if (property.FirstValue == -1 || (property.FirstValue >= overrideData.Min1 && property.FirstValue <= overrideData.Max1))
                                        if (property.SecondValue == -1 || (property.SecondValue >= overrideData.Min2 && property.SecondValue <= overrideData.Max2))
                                        {
                                            try
                                            {
                                                tooltip += string.Format
                                                    (overrideData.FormattedText, property.Name, property.FirstValue.ToString(), property.SecondValue.ToString()) + "\n";

                                                handled = true;

                                                break;
                                            }
                                            catch (System.FormatException e)
                                            {
                                            }
                                        }
                            }
                    }

                    if (!handled) //Did not find a matching override, need to add the plain tooltip line still
                        tooltip += $"{property.OriginalString}\n";
                }

                return tooltip;
            }

            return null;
        }

        private static bool CheckLayers(TooltipLayers overrideLayer, byte itemLayer)
        {
            if ((byte)overrideLayer == itemLayer)
                return true;

            if (overrideLayer == TooltipLayers.Body_Group)
            {
                if (itemLayer == (byte)Layer.Shoes || itemLayer == (byte)Layer.Pants || itemLayer == (byte)Layer.Shirt || itemLayer == (byte)Layer.Helmet ||
                    itemLayer == (byte)Layer.Necklace || itemLayer == (byte)Layer.Arms || itemLayer == (byte)Layer.Gloves || itemLayer == (byte)Layer.Waist ||
                    itemLayer == (byte)Layer.Torso || itemLayer == (byte)Layer.Tunic || itemLayer == (byte)Layer.Legs || itemLayer == (byte)Layer.Skirt ||
                    itemLayer == (byte)Layer.Cloak || itemLayer == (byte)Layer.Robe)
                    return true;
            }
            else if (overrideLayer == TooltipLayers.Jewelry_Group)
            {
                if (itemLayer == (byte)Layer.Talisman || itemLayer == (byte)Layer.Bracelet || itemLayer == (byte)Layer.Ring || itemLayer == (byte)Layer.Earrings)
                    return true;
            }
            else if (overrideLayer == TooltipLayers.Weapon_Group)
            {
                if (itemLayer == (byte)Layer.OneHanded || itemLayer == (byte)Layer.TwoHanded)
                    return true;
            }

            return false;
        }
    }
}