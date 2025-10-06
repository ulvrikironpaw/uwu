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
            Jotunn.Logger.LogInfo($"{Manifest.PluginName} is alive");

            features = new UWUFeature[]
            {
                new ModerBoatingFeature(),
                new NotMyShipFeature(),
                new PaddleFasterFeature(),
                new SailFasterFeature(),
                new SailingGraceFeature(),
                new ShipBonkiesFeature(),
                new ShipPinFeature(),
                new SpeedometerFeature(),
            };

            foreach (var feature in features)
            {
                feature.Configure(Config);
            }
        }

        internal void Update()
        {
            foreach (var feature in features)
            {
                feature.Update();
            }
        }

        internal void LateUpdate()
        {
            foreach (var feature in features)
            {
                feature.LateUpdate();
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
                feature.Destroy();
            }
        }
    }
}