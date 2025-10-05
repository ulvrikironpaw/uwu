using BepInEx;
using Jotunn.Utils;
using UWU.Common;
using UWU.Features;

namespace UWU
{
    [BepInPlugin(Manifest.PluginGUID, Manifest.PluginName, Manifest.PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class UWUMod : BaseUnityPlugin
    {
        private UWUFeature[] features = new UWUFeature[0];

        internal void Awake()
        {
            Jotunn.Logger.LogInfo($"{Manifest.PluginName} is active");

            features = new UWUFeature[]
            {
                new SpeedometerFeature(),
                new SailFasterFeature(),
                new PaddleFasterFeature(),
                new SailingGraceFeature(),
                new NotMyShipFeature(),
                new ShipBonkiesFeature(),
                new ShipPinFeature(),
                new ModerBoatingFeature(),
            };

            foreach (var feature in features)
            {
                feature.Configure(Config);
            }
            foreach (var feature in features)
            {
                feature.Patch();
            }
        }

        internal void Update()
        {
            foreach (var feature in features)
            {
                feature.Update();
            }
        }

        internal void OnGUI()
        {
            if (Hud.IsUserHidden()) return;

            foreach (var feature in features)
            {
                feature.UpdateGUI();
            }
        }

        internal void OnDestroy()
        {
            foreach (var feature in features)
            {
                feature.Unpatch();
            }
        }
    }
}