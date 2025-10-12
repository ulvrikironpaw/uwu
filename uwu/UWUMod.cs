using BepInEx;
using Jotunn.Utils;
using System.Collections;
using UWU.Common;
using UWU.Features;

namespace UWU
{
    [BepInPlugin(Manifest.PluginGUID, Manifest.PluginName, Manifest.PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class UWUMod : BaseUnityPlugin
    {
        private static bool isConfigured = false;
        private static UWUFeature[] features = new UWUFeature[0];

        void Awake()
        {
            if (!isConfigured)
            {
                isConfigured = true;

                features = new UWUFeature[]
                {
                    new BoatyMcBoatfaceFeature(),
                    new ModerBoatingFeature(),
                    new NotMyShipFeature(),
                    new PaddleFasterFeature(),
                    new SailFasterFeature(),
                    new SailingGraceFeature(),
                    new ShipBonkiesFeature(),
                    new ShipNameplateFeature(),
                    new ShipPinFeature(),
                    new ShipRenameFeature(),
                    new SpeedometerFeature(),
                };

                foreach (var feature in features)
                {
                    try
                    {
                        feature.Configure(Config);
                    }
                    catch (System.Exception ex)
                    {
                        Jotunn.Logger.LogError(ex);
                    }
                }
            }
        }

        IEnumerator Start()
        {
            yield return RPCManager.RegisterRPCs();
        }

        void FixedUpdate()
        {
            foreach (var feature in features)
            {
                feature.Update();
            }
        }

        void OnGUI()
        {
            foreach (var feature in features)
            {
                feature.UpdateGUI();
            }
        }
    }
}