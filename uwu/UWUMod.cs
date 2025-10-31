using BepInEx;
using HarmonyLib;
using Jotunn.Utils;
using UnityEngine;
using UWU.Behaviors;
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
    private static FeatureBehaviour[] features = new FeatureBehaviour[0];
    private static GameObject featureHost;

    void Awake()
    {
      if (isConfigured) return;
      isConfigured = true;
      Harmony.CreateAndPatchAll(typeof(ZNet_Awake_Patch));

      featureHost = new GameObject("Features_UWU");
      DontDestroyOnLoad(featureHost);
      features = CreateFeatures();

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

    private static FeatureBehaviour[] CreateFeatures()
    {
      return new FeatureBehaviour[]
      {
        CreateFeature<BoatyMcBoatfaceFeature>(),
        CreateFeature<ModerBoatingFeature>(),
        CreateFeature<NotMyShipFeature>(),
        CreateFeature<PaddleFasterFeature>(),
        CreateFeature<SailFasterFeature>(),
        CreateFeature<SailingGraceFeature>(),
        CreateFeature<ShipBonkiesFeature>(),
        CreateFeature<ShipNameplateFeature>(),
        CreateFeature<ShipPinFeature>(),
        CreateFeature<ShipRenameFeature>(),
        CreateFeature<MoodifierFeature>(),
        CreateFeature<SpeedometerFeature>()
      };
    }

    private static T CreateFeature<T>() where T : FeatureBehaviour
    {
      return featureHost.AddComponent<T>();
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Awake))]
    public static class ZNet_Awake_Patch
    {
      private static void Postfix()
      {
        RPCManager.RegisterRPCs();
      }
    }
  }
}