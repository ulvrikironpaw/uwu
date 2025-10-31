using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using UWU.Behaviors;
using UWU.Commands;
using UWU.Extensions;

namespace UWU.Features
{
  internal sealed class PaddleFasterFeature : FeatureBehaviour
  {
    private static PaddleFasterFeature instance;

    protected override string Name => "PaddleFaster";
    protected override string Category => "Sailing";
    protected override string Description => "Makes paddling forward/backward about twice as fast";

    internal PaddleFasterFeature()
    {
      instance = this;
    }

    // The value to use for paddle backward.
    private float forceBackward = 1.1f;
    // The value to use for paddle forward.
    private float forceForward = 1.2f;

    protected override void OnConfigure(ConfigFile config)
    {
      CommandManager.Instance.AddConsoleCommand(new FloatCommand(
          name: "UWUPaddleForce",
          help: "Changes the rate of paddling. Toggle PaddleFaster to reset.",
          adminOnly: true,
          isCheat: true,
          () => forceBackward,
          (value) =>
          {
            forceBackward = value;
            forceForward = value;
          }));
    }

    protected override void OnPatch(Harmony harmony)
    {
      var original = AccessTools.Method(
        typeof(Ship),
        nameof(Ship.CustomFixedUpdate));
      var prefix = AccessTools.Method(
        typeof(PaddleFasterFeature),
        nameof(Ship_CustomFixedUpdate_Prefix));
      var postfix = AccessTools.Method(
        typeof(PaddleFasterFeature),
        nameof(Ship_CustomFixedUpdate_Postfix));
      harmony.Patch(original, prefix: new(prefix), postfix: new(postfix));
    }

    private static void Ship_CustomFixedUpdate_Prefix(Ship __instance, out Ship_CustomFixedUpdate_State __state)
    {
      __state = new Ship_CustomFixedUpdate_State { initialBackwardForce = __instance.m_backwardForce };

      var speed = __instance.GetSpeed_UWU();
      __instance.m_backwardForce = (__instance.m_backwardForce * 1.5f) * speed switch
      {
        Ship.Speed.Back => instance.forceBackward,
        _ => instance.forceForward,
      };
    }

    private static void Ship_CustomFixedUpdate_Postfix(Ship __instance, Ship_CustomFixedUpdate_State __state)
    {
      __instance.m_backwardForce = __state.initialBackwardForce;
    }

    /// <summary>
    /// Data required to reset the m_backwardForce between calls
    /// </summary>
    struct Ship_CustomFixedUpdate_State
    {
      internal float initialBackwardForce;
    }
  }
}