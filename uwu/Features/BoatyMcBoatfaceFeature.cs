using BepInEx;
using HarmonyLib;
using System;
using UWU.Common;

namespace UWU.Features
{
    internal class BoatyMcBoatfaceFeature : UWUFeature
    {

        private static readonly string[] Prefixes = new string[]
        {
                "Axe", "Bear", "Wolf", "Ice", "Blood", "Storm",
                "Iron", "Frost", "Fire", "Bone", "Steel", "Ghost",
                "Odin", "Thor", "Rune", "Skull", "Night", "Shadow",
                "Grim", "Rage", "Wind", "Snow", "Drake", "Black",
                "Red", "Deep", "Long", "Dark", "Bright", "Rock",
                "Sea", "Stone", "Ash", "Silent", "Thunder", "Sun",
                "Moon", "Wild", "True", "Strong", "Old", "Young",
                "Swift", "Brave", "Mighty", "Hard", "Cold", "Glory",
                "Moon"
        };

        private static readonly string[] Suffixes = new string[]
        {
                "breaker", "singer", "walker", "runner", "seeker", "howler",
                "slayer", "hunter", "rider", "smasher", "warrior", "sailor",
                "weaver", "caller", "watcher", "shaper", "eater", "guard",
                "drinker", "burner", "binder", "reader", "forger", "smith",
                "storm", "fang", "helm", "eye", "blade", "horn",
                "shield", "maw", "claw", "voice", "shade", "soul",
                "beard", "friend", "foe", "hand", "foot", "heart",
                "mark", "blood", "rage", "song", "spell", "roar",
                "moon"
        };


        private static BoatyMcBoatfaceFeature instance;

        internal BoatyMcBoatfaceFeature()
        {
            instance = this;
        }

        internal override string Name => "BoatyMcBoatface";
        protected override string Category => "Sailing";
        protected override string Description => "Automatically give ships names on creation.";
        protected override bool EnabledByDefault => true;


        protected override void OnPatch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Ship), "Start");
            var prefix = AccessTools.Method(typeof(BoatyMcBoatfaceFeature), nameof(Ship_Start_Patch));
            harmony.Patch(original, prefix: new(prefix));
        }

        private static void Ship_Start_Patch(Ship __instance)
        {
            var zNetView = __instance.GetComponent<ZNetView>();
            if (zNetView == null || !zNetView.IsValid() || !zNetView.IsOwner()) return;

            var zdo = zNetView.GetZDO();
            if (zdo == null) return;

            if (!ObjectUtils.GetCustomLabelFromZDO(zdo).IsNullOrWhiteSpace()) return;

            var name = GenerateStableName(zdo.m_uid);
            RPCManager.RenameObject(zdo, name);
        }

        private static string GenerateStableName(ZDOID id)
        {
            long seed = id.UserID ^ id.ID;
            var rng = new Random((int)(seed & 0xFFFFFFFF) ^ (int)(seed >> 32));
            return Prefixes[rng.Next(Prefixes.Length)] + Suffixes[rng.Next(Suffixes.Length)];
        }
    }
}