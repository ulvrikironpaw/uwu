using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;
using UWU.Behaviors;
using UWU.Commands;

namespace UWU.Features
{
  internal sealed class SailingGraceFeature : FeatureBehaviour
  {
    private static SailingGraceFeature instance;

    protected override string Name => "SailingGrace";
    protected override string Category => "Sailing";
    protected override string Description => "Significantly reduces the penalty for headwinds";

    // The denominator used to reduce the speed when on the edges of a headwind.
    private float headwindReductionMin = 5.8f;
    // The denominator used to reduce the speed when in a headwind.
    private float headwindReductionMax = 7.2f;

    internal SailingGraceFeature()
    {
      instance = this;
    }

    protected override void OnConfigure(ConfigFile config)
    {
      CommandManager.Instance.AddConsoleCommand(new FloatCommand(
          name: "UWUHWRMin",
          help: "For debugging, the factor to reduce speed when at the minimum headwind",
          adminOnly: true,
          isCheat: true,
          () => headwindReductionMin,
          (value) => headwindReductionMin = value));
      CommandManager.Instance.AddConsoleCommand(new FloatCommand(
          name: "UWUHWRMax",
          help: "For debugging, the factor to reduce speed when at the maximum headwind",
          adminOnly: true,
          isCheat: true,
          () => headwindReductionMax,
          (value) => headwindReductionMax = value));
    }

    protected override void OnPatch(Harmony harmony)
    {
      var original = AccessTools.Method(
        typeof(Ship),
        "GetSailForce");
      var prefix = AccessTools.Method(
        typeof(SailingGraceFeature),
        nameof(Ship_GetSailForce_Prefix));
      harmony.Patch(original, prefix: new(prefix));
    }

    private static bool Ship_GetSailForce_Prefix(Ship __instance, float sailSize, ref Vector3 __result)
    {
      // Grab the relative angle of the wind to the boat and the wind direction.
      Vector3 windDir = EnvMan.instance.GetWindDir();
      float angle = Vector3.Angle(windDir, __instance.transform.forward);

      Vector3 target;
      if (angle > 135 && angle < 225)
      {
        // Provide some boost even in the wrong direction. It is a game and no one expects it
        // to be accurate to sailing.
        float offset = Mathf.Abs(angle - 180f);
        float percentile = offset / 45f;
        float factor = Mathf.Lerp(instance.headwindReductionMax, instance.headwindReductionMin, percentile);
        target = Vector3.Normalize(__instance.transform.forward) * (__instance.m_sailForceFactor * sailSize / factor);
      }
      else
      {
        // This is an approximation of the original logic.
        float windIntensity = Mathf.Lerp(0.25f, 1f, EnvMan.instance.GetWindIntensity());
        float windAngleFactor = __instance.GetWindAngleFactor() * windIntensity;
        target = Vector3.Normalize(windDir + __instance.transform.forward) * (windAngleFactor * __instance.m_sailForceFactor * sailSize);
      }

      // Create traversers to set/get private fields.
      var traverser = Traverse.Create(__instance);
      var sailForceField = traverser.Field<Vector3>("m_sailForce");
      var windChangeVelocityField = traverser.Field<Vector3>("m_windChangeVelocity");
      // Update the position of the boat.
      var windChangeVelocity = windChangeVelocityField.Value;
      var sailForce = Vector3.SmoothDamp(sailForceField.Value, target, ref windChangeVelocity, 1f, 99f);
      // Update the stored values for wind and sail force.
      windChangeVelocityField.Value = windChangeVelocity;
      sailForceField.Value = sailForce;
      __result = sailForce;

      return false;
    }
  }
}