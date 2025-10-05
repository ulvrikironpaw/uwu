using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using UWU.Common;

namespace UWU.Features
{
    internal sealed class SailFasterFeature : UWUFeature
    {
        private static SailFasterFeature instance;

        internal override string Name => "SailFaster";
        protected override string Category => "Sailing";
        protected override string Description => "Makes ship sail speed about 40% faster.";

        // The default value of full force.
        private const float MAST_COEFFICIENT_DEFAULT = 1f;
        // The full force when at half mast. This is higher so there is a curve.
        private float MAST_COEFFICIENT_HALF = 1.8f;
        // The full force when at full mast. This is lower so there is a curve.
        private float MAST_COEFFICIENT_FULL = 1.6f;

        internal SailFasterFeature()
        {
            instance = this;
        }

        protected override void OnConfigure(ConfigFile config)
        {
            CommandManager.Instance.AddConsoleCommand(new FloatCommand(
                name: "UWUMCHalf",
                help: "For debugging, The multiplier of sailforce when at half mast",
                adminOnly: true,
                isCheat: true,
                () => MAST_COEFFICIENT_HALF,
                (value) => MAST_COEFFICIENT_HALF = value));
            CommandManager.Instance.AddConsoleCommand(new FloatCommand(
                name: "UWUMCFull",
                help: "For debugging, The multiplier of sailforce when at full mast",
                adminOnly: true,
                isCheat: true,
                () => MAST_COEFFICIENT_FULL,
                (value) => MAST_COEFFICIENT_FULL = value));
        }

        protected override void OnPatch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Ship), nameof(Ship.CustomFixedUpdate));
            var prefix = AccessTools.Method(typeof(SailFasterFeature), nameof(Ship_CustomFixedUpdate_Prefix));
            var postfix = AccessTools.Method(typeof(SailFasterFeature), nameof(Ship_CustomFixedUpdate_Postfix));
            harmony.Patch(original, prefix: new(prefix), postfix: new(postfix));
        }

        private static void Ship_CustomFixedUpdate_Prefix(Ship __instance, out Ship_CustomFixedUpdate_State __state)
        {
            // Save the current sailForceFactor so it can be restored after update.
            // This is how different ships with different sale force factors are handled.
            __state = new Ship_CustomFixedUpdate_State { initialSailForceFactor = __instance.m_sailForceFactor };


            // Apply a coefficient to sailing speed if at full or half mast.
            // This makes the speed of the boat significantly faster.
            if (instance.Enabled.Value)
            {
                // Get the current speed from the internal instance. This is gross.
                var speed = Traverse.Create(__instance).Field<Ship.Speed>("m_speed").Value;
                __instance.m_sailForceFactor *= speed switch
                {
                    Ship.Speed.Full => instance.MAST_COEFFICIENT_FULL,
                    Ship.Speed.Half => instance.MAST_COEFFICIENT_HALF,
                    _ => MAST_COEFFICIENT_DEFAULT,
                };
            }
        }

        private static void Ship_CustomFixedUpdate_Postfix(Ship __instance, Ship_CustomFixedUpdate_State __state)
        {
            if (instance.Enabled.Value)
            {
                // Restore the original value after calculations take place.
                __instance.m_sailForceFactor = __state.initialSailForceFactor;
            }
        }

        /// <summary>
        /// Data required to reset the sailForceFactor between calls
        /// </summary>
        struct Ship_CustomFixedUpdate_State
        {
            public float initialSailForceFactor;
        }
    }
}