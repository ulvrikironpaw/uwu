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

            SpeedometerFeature.Configure(Config);
            SpeedometerFeature.Patch(harmony);

            SailingAdjustmentFeature.Configure(Config);
            SailingAdjustmentFeature.Patch(harmony);

            ShipBonkiesFeature.Configure(Config);
            ShipBonkiesFeature.Patch(harmony);

            SailPinFeature.Configure(Config);
        }

        internal void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        internal void OnGUI()
        {
            if (!Hud.IsUserHidden())
            {
                SpeedometerFeature.OnGUI();
            }
        }

        internal void Update()
        {
            SailPinFeature.Update();
        }
    }
}