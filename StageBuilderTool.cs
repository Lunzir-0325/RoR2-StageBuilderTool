using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace StageBuilderTool
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(GUID, modname, modver)]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class StageRebuilder : BaseUnityPlugin
    {
        public const string GUID = "com.Lunzir.StageBuilderTool", modname = "StageBuilderTool", modver = "1.0.0";
        private static List<Card> Cards = new List<Card>();

        public void Awake()
        {
            ModConfig.InitConfig(Config);
            
            if (ModConfig.EnableMod.Value)
            {
                On.RoR2.Run.Start += Run_Start;
                On.RoR2.SceneDirector.PlaceTeleporter += SceneDirector_PlaceTeleporter;
                On.RoR2.SceneDirector.GenerateInteractableCardSelection += SceneDirector_GenerateInteractableCardSelection;
            }
        }
        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            if (ModConfig.EnableMod.Value)
            {
                Config.Reload();
                ModConfig.InitConfig(Config);
                InitData();
            }
            orig(self);
        }
        private void InitData()
        {
            if (ModConfig.EnableWeight.Value)
            {
                Cards.Clear();
                Cards.Add(new Card("Barrel1", ModConfig.Barrel1Weight.Value));
                Cards.Add(new Card("Drone1Broken", ModConfig.Drone1BrokenWeight.Value));
                Cards.Add(new Card("Drone2Broken", ModConfig.Drone2BrokenWeight.Value));
                Cards.Add(new Card("EmergencyDroneBroken", ModConfig.EmergencyDroneBrokenWeight.Value));
                Cards.Add(new Card("EquipmentDroneBroken", ModConfig.EquipmentDroneBrokenWeight.Value));
                Cards.Add(new Card("FlameDroneBroken", ModConfig.FlameDroneBrokenWeight.Value));
                Cards.Add(new Card("MegaDroneBroken", ModConfig.MegaDroneBrokenWeight.Value));
                Cards.Add(new Card("MissileDroneBroken", ModConfig.MissileDroneBrokenWeight.Value));
                Cards.Add(new Card("Turret1Broken", ModConfig.Turret1BrokenWeight.Value));
                Cards.Add(new Card("CasinoChest", ModConfig.CasinoChestWeight.Value));
                Cards.Add(new Card("CategoryChestDamage", ModConfig.CategoryChestDamageWeight.Value));
                Cards.Add(new Card("CategoryChestHealing", ModConfig.CategoryChestHealingWeight.Value));
                Cards.Add(new Card("CategoryChestUtility", ModConfig.CategoryChestUtilityWeight.Value));
                Cards.Add(new Card("CategoryChest2Damage Variant", ModConfig.CategoryChest2DamageWeight.Value));
                Cards.Add(new Card("CategoryChest2Healing Variant", ModConfig.CategoryChest2HealingWeight.Value));
                Cards.Add(new Card("CategoryChest2Utility Variant", ModConfig.CategoryChest2UtilityWeight.Value));
                Cards.Add(new Card("Chest1StealthedVariant", ModConfig.Chest1StealthedWeight.Value));
                Cards.Add(new Card("Chest1", ModConfig.Chest1Weight.Value));
                Cards.Add(new Card("Chest2", ModConfig.Chest2Weight.Value));
                Cards.Add(new Card("Duplicator", ModConfig.DuplicatorWeight.Value));
                Cards.Add(new Card("DuplicatorLarge", ModConfig.DuplicatorLargeWeight.Value));
                Cards.Add(new Card("DuplicatorMilitary", ModConfig.DuplicatorMilitaryWeight.Value));
                Cards.Add(new Card("DuplicatorWild", ModConfig.DuplicatorWildWeight.Value));
                Cards.Add(new Card("EquipmentBarrel", ModConfig.EquipmentBarrelWeight.Value));
                Cards.Add(new Card("GoldChest", ModConfig.GoldChestWeight.Value));
                Cards.Add(new Card("LunarChest", ModConfig.LunarChestWeight.Value));
                Cards.Add(new Card("Scrapper", ModConfig.ScrapperWeight.Value));
                Cards.Add(new Card("ShrineBlood", ModConfig.ShrineBloodWeight.Value));
                Cards.Add(new Card("ShrineBoss", ModConfig.ShrineBossWeight.Value));
                Cards.Add(new Card("ShrineChance", ModConfig.ShrineChanceWeight.Value));
                Cards.Add(new Card("ShrineCleanse", ModConfig.ShrineCleanseWeight.Value));
                Cards.Add(new Card("ShrineCombat", ModConfig.ShrineCombatWeight.Value));
                Cards.Add(new Card("ShrineGoldshoresAccess", ModConfig.ShrineGoldshoresAccessWeight.Value));
                Cards.Add(new Card("ShrineHealing", ModConfig.ShrineHealingWeight.Value));
                Cards.Add(new Card("ShrineRestack", ModConfig.ShrineRestackWeight.Value));
                Cards.Add(new Card("TripleShop", ModConfig.TripleShopWeight.Value));
                Cards.Add(new Card("TripleShopEquipment", ModConfig.TripleShopEquipmentWeight.Value));
                Cards.Add(new Card("TripleShopLarge", ModConfig.TripleShopLargeWeight.Value));
                Cards.Add(new Card("VoidCamp", ModConfig.VoidCampWeight.Value));
                Cards.Add(new Card("VoidChest", ModConfig.VoidChestWeight.Value)); 
            }
        }
        private void SceneDirector_PlaceTeleporter(On.RoR2.SceneDirector.orig_PlaceTeleporter orig, SceneDirector self)
        {
            orig.Invoke(self);
            if (ModConfig.EnableBasicPara.Value)
            {
                self.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
                int playerCount = Run.instance.participatingPlayerCount;
                if (ModConfig.PlayerCount.Value != -1)
                {
                    playerCount = ModConfig.PlayerCount.Value;
                }
                float num = 0.5f + playerCount * 0.5f;
                ClassicStageInfo component = SceneInfo.instance.GetComponent<ClassicStageInfo>();
                if (component)
                {
                    self.interactableCredit = (int)(ModConfig.InteractibleCredits.Value * num);
                    if (component.bonusInteractibleCreditObjects != null)
                    {
                        for (int i = 0; i < component.bonusInteractibleCreditObjects.Length; i++)
                        {
                            ClassicStageInfo.BonusInteractibleCreditObject bonusInteractibleCreditObject = component.bonusInteractibleCreditObjects[i];
                            if (bonusInteractibleCreditObject.objectThatGrantsPointsIfEnabled && bonusInteractibleCreditObject.objectThatGrantsPointsIfEnabled.activeSelf)
                            {
                                self.interactableCredit += bonusInteractibleCreditObject.points;
                            }
                        }
                    }
                    self.monsterCredit = (int)(ModConfig.MonsterCredits.Value * Run.instance.difficultyCoefficient);
                }
            }
        }
        private WeightedSelection<DirectorCard> SceneDirector_GenerateInteractableCardSelection(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, SceneDirector self)
        {
            if (ModConfig.EnableWeight.Value)
            {
                WeightedSelection<DirectorCard> weightedSelection = orig.Invoke(self);
                for (int i = 0; i < weightedSelection.Count; i++)
                {
                    WeightedSelection<DirectorCard>.ChoiceInfo choice = weightedSelection.GetChoice(i);
                    string prefabName = choice.value.spawnCard.prefab.name;
                    //Send($"{choice.value.spawnCard.name}\t{prefabName}\t{choice.weight}");
                    Card card = Cards.FirstOrDefault(x => x.Name.ToLower() == prefabName.ToLower());
                    if (card != null)
                    {
                        choice.weight = card.Weight;
                        weightedSelection.ModifyChoiceWeight(i, card.Weight);
                    }
                }
                return weightedSelection;
            }
            else
            {
                return orig.Invoke(self);
            }
        }

        public static void Send(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = message,

            });
        }

        internal class Card
        {
            public string Name;
            public float Weight;

            public Card(string name, float weight)
            {
                Name = name;
                Weight = weight;
            }
        }

    }
    class ModConfig
    {
        public static ConfigEntry<bool> EnableMod;
        public static ConfigEntry<bool> EnableBasicPara;
        public static ConfigEntry<bool> EnableWeight;
        public static ConfigEntry<int> PlayerCount;
        public static ConfigEntry<int> InteractibleCredits;
        public static ConfigEntry<int> MonsterCredits;
        public static ConfigEntry<float> Barrel1Weight;
        public static ConfigEntry<float> Drone1BrokenWeight;
        public static ConfigEntry<float> Drone2BrokenWeight;
        public static ConfigEntry<float> EmergencyDroneBrokenWeight;
        public static ConfigEntry<float> EquipmentDroneBrokenWeight;
        public static ConfigEntry<float> FlameDroneBrokenWeight;
        public static ConfigEntry<float> MegaDroneBrokenWeight;
        public static ConfigEntry<float> MissileDroneBrokenWeight;
        public static ConfigEntry<float> Turret1BrokenWeight;
        public static ConfigEntry<float> CasinoChestWeight;
        public static ConfigEntry<float> CategoryChestDamageWeight;
        public static ConfigEntry<float> CategoryChestHealingWeight;
        public static ConfigEntry<float> CategoryChestUtilityWeight;
        public static ConfigEntry<float> CategoryChest2DamageWeight;
        public static ConfigEntry<float> CategoryChest2HealingWeight;
        public static ConfigEntry<float> CategoryChest2UtilityWeight;
        public static ConfigEntry<float> Chest1StealthedWeight;
        public static ConfigEntry<float> Chest1Weight;
        public static ConfigEntry<float> Chest2Weight;
        public static ConfigEntry<float> DuplicatorWeight;
        public static ConfigEntry<float> DuplicatorLargeWeight;
        public static ConfigEntry<float> DuplicatorMilitaryWeight;
        public static ConfigEntry<float> DuplicatorWildWeight;
        public static ConfigEntry<float> EquipmentBarrelWeight;
        public static ConfigEntry<float> GoldChestWeight;
        public static ConfigEntry<float> LunarChestWeight;
        public static ConfigEntry<float> ScrapperWeight;
        public static ConfigEntry<float> ShrineBloodWeight;
        public static ConfigEntry<float> ShrineBloodSnowyWeight;
        public static ConfigEntry<float> ShrineBossWeight;
        public static ConfigEntry<float> ShrineBossSnowyWeight;
        public static ConfigEntry<float> ShrineChanceWeight;
        public static ConfigEntry<float> ShrineChanceSnowyWeight;
        public static ConfigEntry<float> ShrineCleanseWeight;
        public static ConfigEntry<float> ShrineCombatWeight;
        public static ConfigEntry<float> ShrineGoldshoresAccessWeight;
        public static ConfigEntry<float> ShrineHealingWeight;
        public static ConfigEntry<float> ShrineRestackWeight;
        public static ConfigEntry<float> TripleShopWeight;
        public static ConfigEntry<float> TripleShopEquipmentWeight;
        public static ConfigEntry<float> TripleShopLargeWeight;
        public static ConfigEntry<float> VoidCampWeight;
        public static ConfigEntry<float> VoidChestWeight;

        public static void InitConfig(ConfigFile config)
        {
            EnableMod = config.Bind("0 Setting设置", "EnableMod", true, "If enable the mod, the configuration file will reload when start a new run, so you can adjust it and dont need restart the game.\n" +
                "启用模组，每次开局会重新加载一次配置文件，方便修改。");
            if (EnableMod.Value)
            {
                EnableBasicPara = config.Bind("1 Basic Parameters基础参数", "EnableBasicParameters", false, "Enable parameter adjustment.If have use the mod ShareSuite, you can close this option.\n" +
                    "启用基础参数修改，以下参数是从游戏本体调出来的，如果你有用共享mod(ShareSuite)的InteractablesCredit选项会有冲突。");
                if (EnableBasicPara.Value)
                {
                    PlayerCount = config.Bind("1 Basic Parameters基础参数", "PlayerCount", -1, "The number of players coefficient only affects the following option - InteractibleCredits. Default is -1 increases automatically with the number of players.\n" +
                        "玩家人数系数，影响下面地图设施，-1 = 默认随玩家人数自动增长");
                    InteractibleCredits = config.Bind("1 Basic Parameters基础参数", "InteractibleCredits", 200, "This option I prefer to think of as capacity. The higher the values, the more facilities will be generated.\n" +
                        "地图交互设施点数，点数越高生成的设施会越多。游戏原本就是200点数，我个人觉得太多了特别后期太多东西会卡。推荐改成100点数体验会更好。");
                    MonsterCredits = config.Bind("1 Basic Parameters基础参数", "MonsterCredits", 20, "Same, prefer to think of as capacity. The higher the number of points, the higher the proportion of small monsters to large monsters, and even into a boss.\n" +
                        "怪物生成点数，随游戏时间增长，点数越高小怪变大怪比重越高，甚至全是boss，但不会因此小怪就不会刷新，小怪的比重变小而已。");

                }
                EnableWeight = config.Bind("2 Weight比重", "EnableWeight", true, "Enable interactive facility generation weight modification.\n" +
                    "Each map has its own facility specific label, and without that, even 100 weights won't work. This part is of little significance, but at least it can removes my favorite Barrel!\n" +
                    "Chests will not appear if sacrifice artifacts are enabled." +
                    "启用交互设施生成比重修改。\n" +
                    "注意：每张图都有自己的特定设施标签，如果没有该标签，就算设定100比重没用，这方面没有深入研究，而且研究意义不大，但至少可以把我最喜欢的钱桶去掉。\n" +
                    "如果用了神器，比如牺牲，宝箱是不会出现的。");
                if (EnableWeight.Value)
                {
                    Barrel1Weight = config.Bind("2 Weight比重", "Barrel1Weight", 10.0f, "Barrel\n钱桶比重");
                    Drone1BrokenWeight = config.Bind("2 Weight比重", "Drone1BrokenWeight", 7f, "Broken Gunner Drone\n损坏的抢手无人机比重");
                    Drone2BrokenWeight = config.Bind("2 Weight比重", "Drone2BrokenWeight", 7f, "Broken Healing Drone\n损坏的治疗无人机比重");
                    EmergencyDroneBrokenWeight = config.Bind("2 Weight比重", "EmergencyDroneBrokenWeight", 1.75f, "Broken Emergency Drone\n损坏的应急无人机比重");
                    EquipmentDroneBrokenWeight = config.Bind("2 Weight比重", "EquipmentDroneBrokenWeight", 2.0f, "Broken Equipment Drone\n损坏的装备无人机比重");
                    FlameDroneBrokenWeight = config.Bind("2 Weight比重", "FlameDroneBrokenWeight", 1.0f, "Broken Incinerator Drone\n损坏的焚烧无人机比重");
                    MegaDroneBrokenWeight = config.Bind("2 Weight比重", "MegaDroneBrokenWeight", 1.5f, "Broken TC-280\n损坏的TC-280比重");
                    MissileDroneBrokenWeight = config.Bind("2 Weight比重", "MissileDroneBrokenWeight", 5.5f, "Broken Missile Drone\n损坏的导弹无人机比重");
                    Turret1BrokenWeight = config.Bind("2 Weight比重", "Turret1BrokenWeight", 7.0f, "Broken Gunner Turret\n损坏的枪手机枪塔比重");
                    CasinoChestWeight = config.Bind("2 Weight比重", "CasinoChestWeight", 1.0f, "Adaptive Chest\n适配宝箱比重");
                    CategoryChestDamageWeight = config.Bind("2 Weight比重", "CategoryChestDamageWeight", 2.0f, "Chest - Damage\n宝箱 - 伤害比重");
                    CategoryChestHealingWeight = config.Bind("2 Weight比重", "CategoryChestHealingWeight", 2.0f, "Chest - Healing\n宝箱 - 治疗比重");
                    CategoryChestUtilityWeight = config.Bind("2 Weight比重", "CategoryChestUtilityWeight", 2.0f, "Chest - Utility\n宝箱 - 辅助比重");
                    CategoryChest2DamageWeight = config.Bind("2 Weight比重", "CategoryChest2DamageWeight", 0.3f, "Chest - Damage(Large)\n大宝箱 - 伤害比重");
                    CategoryChest2HealingWeight = config.Bind("2 Weight比重", "CategoryChest2HealingWeight", 0.3f, "Chest - Healing(Large)\n大宝箱 - 治疗比重");
                    CategoryChest2UtilityWeight = config.Bind("2 Weight比重", "CategoryChest2UtilityWeight", 0.3f, "Chest - Utility(Large)\n大宝箱 - 辅助比重");
                    Chest1StealthedWeight = config.Bind("2 Weight比重", "Chest1StealthedWeight", 0.4f, "Cloaked Chest\n被遮盖的宝箱比重");
                    Chest1Weight = config.Bind("2 Weight比重", "Chest1Weight", 24f, "Chest\n宝箱比重");
                    Chest2Weight = config.Bind("2 Weight比重", "Chest2Weight", 3.7f, "Large Chest\n巨大宝箱比重");
                    DuplicatorWeight = config.Bind("2 Weight比重", "DuplicatorWeight", 5.7f, "3D Printer(Tier 1)\n白色打印机比重");
                    DuplicatorLargeWeight = config.Bind("2 Weight比重", "DuplicatorLargeWeight", 1.0f, "3D Printer(Tier 2)\n绿色打印机比重");
                    DuplicatorMilitaryWeight = config.Bind("2 Weight比重", "DuplicatorMilitaryWeight", 0.2f, "Mili-Tech Printer\n红色打印机比重");
                    DuplicatorWildWeight = config.Bind("2 Weight比重", "DuplicatorWildWeight", 0.32f, "Overgrown 3D Printer\n黄色打印机比重");
                    EquipmentBarrelWeight = config.Bind("2 Weight比重", "EquipmentBarrelWeight", 2.0f, "Equipment Barrel\n武器装备桶比重");
                    GoldChestWeight = config.Bind("2 Weight比重", "GoldChestWeight", 0.08f, "Legendary Chest\n传奇宝箱比重");
                    LunarChestWeight = config.Bind("2 Weight比重", "LunarChestWeight", 1.95f, "Lunar Pod\n月球舱比重");
                    ScrapperWeight = config.Bind("2 Weight比重", "ScrapperWeight", 2.3f, "Scrapper\n收割机比重");
                    ShrineBloodWeight = config.Bind("2 Weight比重", "ShrineBloodWeight", 3.75f, "Shrine of Blood\n鲜血神龛比重");
                    ShrineBossWeight = config.Bind("2 Weight比重", "ShrineBossWeight", 1.25f, "Shrine of the Mountain\n山之神龛比重");
                    ShrineChanceWeight = config.Bind("2 Weight比重", "ShrineChanceWeight", 5f, "Shrine of Chance\n机率神龛比重");
                    ShrineBloodSnowyWeight = config.Bind("2 Weight比重", "ShrineBloodSnowyWeight", 3.75f, "Shrine of Blood(Snowy)\n鲜血神龛比重（雪地图）");
                    ShrineBossSnowyWeight = config.Bind("2 Weight比重", "ShrineBossSnowyWeight", 1.25f, "Shrine of the Mountain(Snowy)\n山之神龛比重（雪地图）");
                    ShrineChanceSnowyWeight = config.Bind("2 Weight比重", "ShrineChanceSnowyWeight", 5f, "Shrine of Chance(Snowy)机率神龛比重（雪地图）");
                    ShrineCleanseWeight = config.Bind("2 Weight比重", "ShrineCleanseWeight", 1.6f, "Cleansing Pool\n净化池比重");
                    ShrineCombatWeight = config.Bind("2 Weight比重", "ShrineCombatWeight", 3.75f, "Shrine of Combat\n战斗神龛比重");
                    ShrineGoldshoresAccessWeight = config.Bind("2 Weight比重", "ShrineGoldshoresAccessWeight", 0.1f, "Altar of Gold\n黄金祭坛比重");
                    ShrineHealingWeight = config.Bind("2 Weight比重", "ShrineHealingWeight", 1.07f, "Shrine of the Woods\n森林神龛比重");
                    ShrineRestackWeight = config.Bind("2 Weight比重", "ShrineRestackWeight", 0.7f, "Shrine of Order\n秩序比重");
                    TripleShopWeight = config.Bind("2 Weight比重", "TripleShopWeight", 7.82f, "Multishop Terminal\n多重商店白装比重");
                    TripleShopEquipmentWeight = config.Bind("2 Weight比重", "TripleShopEquipmentWeight", 2.09f, "Multishop Equipment Terminal\n多重商店主动装备比重");
                    TripleShopLargeWeight = config.Bind("2 Weight比重", "TripleShopLargeWeight", 4.28f, "Multishop Terminal(Tier 2)\n多重商店绿装比重");
                    VoidCampWeight = config.Bind("2 Weight比重", "VoidCampWeight", 0.95f, "Void Seed\n虚空种子比重");
                    VoidChestWeight = config.Bind("2 Weight比重", "VoidChestWeight", 3f, "Void Cradle\n虚空箱比重"); 
                }
            }

        }
    }
}
