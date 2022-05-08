using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;

namespace StageBuilderTool
{
    class Present
    {
        public static void Load()
        {
            if (true)
            {

            }
        }
    }
    class ArtifactStruct
    {
        public static string 恶意神器 = "Bomb";
        public static string 统率神器 = "Command";
        public static string 荣耀神器 = "EliteOnly";
        public static string 谜团神器 = "Enigma";
        public static string 混沌神器 = "FriendlyFire";
        public static string 玻璃神器 = "Glass";
        public static string 纷争神器 = "MixEnemy";
        public static string 进化神器 = "MonsterTeamGainsItems";
        public static string 蜕变神器 = "RandomSurvivorOnRespawn";
        public static string 牺牲神器 = "Sacrifice";
        public static string 复仇神器 = "ShadowClone";
        public static string 亲族神器 = "SingleMonsterType";
        public static string 虫群神器 = "Swarms";
        public static string 亡者神器 = "TeamDeath";
        public static string 脆弱神器 = "WeakAssKnees";
        public static string 灵魂神器 = "WispOnDeath";

        public static List<string> Instance;
        public ArtifactStruct()
        {
            Instance = new List<string>
            {
                恶意神器,
                统率神器,
                荣耀神器,
                谜团神器,
                混沌神器,
                玻璃神器,
                纷争神器,
                进化神器,
                蜕变神器,
                牺牲神器,
                复仇神器,
                亲族神器,
                虫群神器,
                亡者神器,
                脆弱神器,
                灵魂神器
            };

        }
        public static void FindArtifact(string name, out ArtifactDef art, out ArtifactIndex index, out bool enable)
        {
            art = ArtifactCatalog.FindArtifactDef(name);
            index = ArtifactCatalog.FindArtifactIndex(name);
            enable = RunArtifactManager.instance.IsArtifactEnabled(art);
        }
    }
}
