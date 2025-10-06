using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using UWU.Common;

namespace UWU.Features
{
    internal sealed class PaddleFasterFeature : UWUFeature
    {
        private static PaddleFasterFeature instance;

        internal override string Name => "PaddleFaster";
        protected override string Category => "Sailing";
        protected override string Description => "Makes paddling forward/backward about twice as fast";

        internal PaddleFasterFeature()
        {
            instance = this;
        }

        // The vanilla backward force.
        private const float FORCE_DEFAULT = 0.02f;
        // The value to use for paddle backward.
        private float forceBackward = 0.38f;
        // The value to use for paddle forward.
        private float forceForward = 0.38f;

        protected override void OnConfigure(ConfigFile config)
        {
            CommandManager.Instance.AddConsoleCommand(new FloatCommand(
                name: "UWUPaddleForce",
                help: "Changes the rate of paddling. Toggle PaddleFaster to reset.",
                adminOnly: true,
                isCheat: false,
                () => forceBackward,
                (value) =>
                {
                    forceBackward = value;
                    forceForward = value;
                }));
        }

        protected override void OnPatch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Ship), nameof(Ship.CustomFixedUpdate));
            var prefix = AccessTools.Method(typeof(PaddleFasterFeature), nameof(Ship_CustomFixedUpdate_Prefix));
            var postfix = AccessTools.Method(typeof(PaddleFasterFeature), nameof(Ship_CustomFixedUpdate_Postfix));
            harmony.Patch(original, prefix: new(prefix), postfix: new(postfix));
        }

        private static void Ship_CustomFixedUpdate_Prefix(Ship __instance)
        {
            var speed = Traverse.Create(__instance).Field<Ship.Speed>("m_speed").Value;
            __instance.m_backwardForce = speed switch
            {
                Ship.Speed.Back => instance.forceBackward,
                _ => instance.forceForward,
            };
        }

        private static void Ship_CustomFixedUpdate_Postfix(Ship __instance)
        {
            __instance.m_backwardForce = FORCE_DEFAULT;
        }
    }
}