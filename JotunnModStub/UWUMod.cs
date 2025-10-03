using BepInEx;
using HarmonyLib;
using Jotunn.Utils;

namespace UWU
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class UWUMod : BaseUnityPlugin
    {
        public const string PluginGUID = "com.ulvrikironpaw.uwu";
        public const string PluginName = "Ulvrik's World Update";
        public const string PluginVersion = "0.0.1";

        private readonly Harmony harmony = new(PluginGUID);

        internal void Awake()
        {
            Jotunn.Logger.LogInfo($"{PluginName} is active");
            // Configure and patch speedometer.
            SpeedometerFeature.Configure(Config);
            SpeedometerFeature.Patch(harmony);
            // Configure and patch sailing speed adjustments
            SailingSpeedFeature.Configure(Config);
            SailingSpeedFeature.Patch(harmony);
            // Configure and patch whether enemies are aggressive to sailboats.
            ReduceShipAggressionFeature.Configure(Config);
            ReduceShipAggressionFeature.Patch(harmony);
            // Configure and patch the ability to destruct a ship with the hammer.
            ShipBonkiesFeature.Configure(Config);
            ShipBonkiesFeature.Patch(harmony);
            // Configures auto-adding map pins for ships.
            SailPinFeature.Configure(Config);
            // Configures adding permanent buffs.
            PermanentBuffFeature.Configure(Config);
            PermanentBuffFeature.Patch(harmony);
        }

        internal void OnDestroy()
        {
            // Removes harmony patches.
            harmony.UnpatchSelf();
        }

        internal void OnGUI()
        {
            if (Hud.IsUserHidden()) return;
            // Draw the speedometer if necessary.
            SpeedometerFeature.OnGUI();
        }

        internal void Update()
        {
            // Call the Update() function on plugins which are have normal Unity
            // hooks.
            PermanentBuffFeature.Update();
            SailPinFeature.Update();
        }
    }
}