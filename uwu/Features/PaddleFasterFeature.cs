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

        // The value to use for paddle backward.
        private float BACKWARD_FORCE = 0.42f;
        // The value to use for paddle forward.
        private float PADDLE_FORCE = 0.38f;

        protected override void OnConfigure(ConfigFile config)
        {
            CommandManager.Instance.AddConsoleCommand(new FloatCommand(
                name: "UWUPaddleForce",
                help: "The rate of paddling",
                adminOnly: true,
                isCheat: false,
                () => BACKWARD_FORCE,
                (value) =>
                {
                    BACKWARD_FORCE = value;
                    PADDLE_FORCE = value;
                }));
        }

        protected override void OnPatch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Ship), nameof(Ship.CustomFixedUpdate));
            var prefix = AccessTools.Method(typeof(PaddleFasterFeature), nameof(Ship_CustomFixedUpdate_Prefix));
            harmony.Patch(original, prefix: new(prefix));
        }

        private static void Ship_CustomFixedUpdate_Prefix(Ship __instance)
        {
            if (!instance.Enabled.Value) return;

            var speed = Traverse.Create(__instance).Field<Ship.Speed>("m_speed").Value;
            __instance.m_backwardForce = speed switch
            {
                Ship.Speed.Slow => instance.PADDLE_FORCE,
                _ => instance.BACKWARD_FORCE,
            };
        }
    }
}