using HarmonyLib;
using UnityEngine;

namespace UWU
{
    static class SpeedometerFeature
    {

        // This static variable will hold our calculated speed.
        // We make it static so the patch can set it, and the GUI method can read it.
        private static float currentSpeed = 0f;
        private static bool useKnots = false;

        internal static void Apply(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Player), nameof(Player.CustomFixedUpdate));
            var postfix = AccessTools.Method(typeof(SpeedometerFeature), nameof(SpeedometerFeature.Player_CustomFixedUpdate_PostFix));
            harmony.Patch(original, postfix: new(postfix));
        }

        internal static void OnGUI()
        {
            // Only draw the GUI if the player exists and the game HUD is visible.
            if (Player.m_localPlayer == null)
            {
                return;
            }

            // Define the style for our label's text.
            GUIStyle style = new()
            {
                fontSize = 32 // A good readable size
            };
            style.normal.textColor = Color.white; // White text

            // We also change the unit depending on whether the player is on a ship or not.
            string speedText = useKnots
                ? $"Speed: {Mathf.Floor(currentSpeed)} knots"
                : $"Speed: {Mathf.Floor(currentSpeed)} m/s";

            // This will place it 10 pixels from the top and 10 from the left.
            Rect labelRect = new(10, 10, 250, 40);

            // Draw a semi-transparent background for better readability
            Color backgroundColor = new(0, 0, 0, 0.5f); // Black with 50% opacity
            GUI.backgroundColor = backgroundColor;
            GUI.Box(labelRect, GUIContent.none);

            // Draw the actual label with the speed text.
            GUI.Label(labelRect, speedText, style);
        }

        private const float SMOOTHING_FACTOR = 1f;

        static void Player_CustomFixedUpdate_PostFix(Player __instance, float fixedDeltaTime)
        {
            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            // Check if the player is controlling a ship.
            Ship controlledShip = __instance.GetControlledShip();
            if (controlledShip != null)
            {
                // The ship has its own speed calculation.
                // Ship speed is roughly in knots, so we'll label it as such.
                var knots = controlledShip.GetSpeed();
                currentSpeed = !useKnots ? knots : Mathf.Lerp(currentSpeed, knots, fixedDeltaTime * SMOOTHING_FACTOR);
                useKnots = true;
            }
            else
            {
                // If not on a ship, get the player's physical body's velocity.
                // The velocity is a Vector3, so we get its magnitude for a single speed value.
                // This value is in meters per second (m/s).
                var metersPerSecond = __instance.GetVelocity().magnitude;
                currentSpeed = useKnots ? currentSpeed : Mathf.Lerp(currentSpeed, metersPerSecond, fixedDeltaTime * SMOOTHING_FACTOR);
                useKnots = false;
            }
        }
    }

}