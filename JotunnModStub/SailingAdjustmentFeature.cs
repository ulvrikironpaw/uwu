using HarmonyLib;
using UnityEngine;

namespace UWU
{
    public static class SailingAdjustmentFeature
    {
        // The value to use for paddle backward.
        private const float BACKWARD_FORCE = 0.4f;
        // The value to use for paddle forward.
        private const float PADDLE_FORCE = 0.4f;
        // The denominator used to reduce the speed when on the edges of a headwind.
        private const float HEADWIND_REDUCTION_FACTOR_MIN = 5.8f;
        // The denominator used to reduce the speed when in a headwind.
        private const float HEADWIND_REDUCTION_FACTOR_MAX = 7.2f;
        // The default value of full force.
        private const float MAST_COEFFICIENT_DEFAULT = 1f;
        // The full force when at half mast. This is higher so there is a curve.
        private const float MAST_COEFFICIENT_HALF = 1.8f;
        // The full force when at full mast. This is lower so there is a curve.
        private const float MAST_COEFFICIENT_FULL = 1.4f;

        internal static void Apply(Harmony harmony)
        {
            ApplySailForceUpdates(harmony);
            ApplyHeadWindUpdates(harmony);
        }

        private static void ApplySailForceUpdates(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Ship), "GetSailForce");
            var prefix = AccessTools.Method(typeof(SailingAdjustmentFeature), nameof(Ship_GetSailForce_Prefix));
            harmony.Patch(original, prefix: new(prefix));
        }

        private static void ApplyHeadWindUpdates(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Ship), nameof(Ship.CustomFixedUpdate));
            var prefix = AccessTools.Method(typeof(SailingAdjustmentFeature), nameof(Ship_CustomFixedUpdate_Prefix));
            var postfix = AccessTools.Method(typeof(SailingAdjustmentFeature), nameof(Ship_CustomFixedUpdate_Postfix));
            harmony.Patch(original, prefix: new(prefix), postfix: new(postfix));
        }

        static bool Ship_GetSailForce_Prefix(Ship __instance, float sailSize, ref Vector3 __result)
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
                float factor = Mathf.Lerp(HEADWIND_REDUCTION_FACTOR_MAX, HEADWIND_REDUCTION_FACTOR_MIN, percentile);
                target = Vector3.Normalize(__instance.transform.forward) * ((__instance.m_sailForceFactor * sailSize) / factor);
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

        static void Ship_CustomFixedUpdate_Prefix(Ship __instance, out Ship_CustomFixedUpdate_State __state)
        {
            // Save the current sailForceFactor so it can be restored after update.
            // This is how different ships with different sale force factors are handled.
            __state = new Ship_CustomFixedUpdate_State { initialSailForceFactor = __instance.m_sailForceFactor };

            // Get the current speed from the internal instance. This is gross.
            var speed = Traverse.Create(__instance).Field<Ship.Speed>("m_speed").Value;

            // Apply a coefficient to sailing speed if at full or half mast.
            // This makes the speed of the boat significantly faster.
            __instance.m_sailForceFactor *= speed switch
            {
                Ship.Speed.Full => MAST_COEFFICIENT_FULL,
                Ship.Speed.Half => MAST_COEFFICIENT_HALF,
                _ => MAST_COEFFICIENT_DEFAULT,
            };

            // m_backwardForce is used to handle both paddle and backwards speed.
            // Switch between speeds based on the current "speed".
            __instance.m_backwardForce = speed switch
            {
                Ship.Speed.Slow => PADDLE_FORCE,
                _ => BACKWARD_FORCE,
            };
        }

        static void Ship_CustomFixedUpdate_Postfix(Ship __instance, Ship_CustomFixedUpdate_State __state)
        {
            // Restore the original value after calculations take place.
            __instance.m_sailForceFactor = __state.initialSailForceFactor;
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