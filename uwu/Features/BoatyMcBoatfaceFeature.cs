using BepInEx;
using HarmonyLib;
using Jotunn.Extensions;
using System;
using UWU.Behaviors;
using UWU.Common;

namespace UWU.Features
{
  internal class BoatyMcBoatfaceFeature : FeatureBehaviour
  {
    private static BoatyMcBoatfaceFeature instance;

    internal BoatyMcBoatfaceFeature()
    {
      instance = this;
    }

    protected override string Name => "BoatyMcBoatface";
    protected override string Category => "Sailing";
    protected override string Description => "Automatically give ships names on creation.";
    protected override bool EnabledByDefault => true;


    protected override void OnPatch(Harmony harmony)
    {
      var original = AccessTools.Method(
        typeof(Ship),
        "Start");
      var prefix = AccessTools.Method(
        typeof(BoatyMcBoatfaceFeature),
        nameof(Ship_Start_Patch));
      harmony.Patch(original, prefix: new(prefix));
    }

    private static void Ship_Start_Patch(Ship __instance)
    {
      var zNetView = __instance.GetComponent<ZNetView>();
      if (zNetView == null || !zNetView.IsValid() || !zNetView.IsOwner())
        return;

      var zdo = zNetView.GetZDO();
      if (zdo == null) return;

      if (!NameCache.GetCustomLabelFromZDO(zdo).IsNullOrWhiteSpace()) return;

      var name = GenerateStableName(zdo.m_uid);
      RPCManager.RenameObject(zdo, name);
    }

    private static string GenerateStableName(ZDOID id)
    {
      long seed = id.UserID ^ id.ID;
      var rng = new Random((int)(seed & 0xFFFFFFFF) ^ (int)(seed >> 32));
      var prefix = Prefixes[rng.Next(Prefixes.Length)];
      var suffix = Suffixes[rng.Next(Suffixes.Length)];
      if (prefix.ToLower() == suffix.ToLower())
      {
        return $"{prefix} {suffix.CapitalizeFirstLetter()}";
      }
      return prefix + suffix;
    }

    private static readonly string[] Prefixes = new string[]
    {
      "Amber",
      "Ash",
      "Axe",
      "Battle",
      "Bear",
      "Black",
      "Blaze",
      "Blood",
      "Bone",
      "Brave",
      "Bright",
      "Cinder",
      "Cold",
      "Crow",
      "Dark",
      "Dawn",
      "Death",
      "Deep",
      "Drake",
      "Dusk",
      "Eagle",
      "Ember",
      "Fire",
      "Flame",
      "Frost",
      "Ghost",
      "Giant",
      "Glory",
      "Gore",
      "Grim",
      "Hard",
      "Hawk",
      "Ice",
      "Iron",
      "Ironfist",
      "Lone",
      "Long",
      "Mighty",
      "Mist",
      "Moon",
      "Night",
      "Oak",
      "Odin",
      "Old",
      "One",
      "Pale",
      "Rage",
      "Raven",
      "Red",
      "Rock",
      "Rune",
      "Sea",
      "Shadow",
      "Silent",
      "Skald",
      "Skull",
      "Snow",
      "Steel",
      "Stone",
      "Storm",
      "Strong",
      "Sun",
      "Swift",
      "Thor",
      "Thunder",
      "True",
      "Valkyr",
      "Wild",
      "Wind",
      "Wolf",
      "Wyrm",
      "Young",
    };

    private static readonly string[] Suffixes = new string[]
    {
      "bane",
      "beard",
      "bearer",
      "binder",
      "biter",
      "blade",
      "blaze",
      "blood",
      "breaker",
      "burner",
      "caller",
      "claw",
      "cleaver",
      "crusher",
      "dance",
      "doom",
      "drinker",
      "eater",
      "eye",
      "fang",
      "fist",
      "flame",
      "flayer",
      "foe",
      "foot",
      "forger",
      "friend",
      "fury",
      "gaze",
      "glare",
      "grim",
      "guard",
      "hand",
      "heart",
      "helm",
      "horn",
      "howler",
      "hunter",
      "lash",
      "maiden",
      "mark",
      "mast",
      "mauler",
      "maw",
      "moon",
      "rage",
      "reader",
      "rider",
      "roamer",
      "roar",
      "runner",
      "sail",
      "sailor",
      "scar",
      "seeker",
      "shade",
      "shaper",
      "shield",
      "singer",
      "slayer",
      "smasher",
      "smith",
      "song",
      "soul",
      "spell",
      "storm",
      "voice",
      "walker",
      "warden",
      "warrior",
      "watcher",
      "weaver",
    };
  }
}