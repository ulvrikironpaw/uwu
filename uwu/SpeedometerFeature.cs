using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Extensions;
using Jotunn.Managers;
using UnityEngine;

namespace UWU
{
    static class SpeedometerFeature
    {

        // This static variable will hold our calculated speed.
        // We make it static so the patch can set it, and the GUI method can read it.
        private static float currentSpeed = 0f;
        private static float updateTimer = 0f;
        private const float maxTime = 0.25f;

        private static ConfigEntry<bool> EnableSpeedometer;

        internal static void Configure(ConfigFile config)
        {
            EnableSpeedometer = config.BindConfig(
                section: "Sailing",
                key: "Speedometer",
                defaultValue: false,
                description: "If enabled, a speedometer will appear on the screen",
                synced: false
            );

            CommandManager.Instance.AddConsoleCommand(new BoolConsoleCommand(
                name: "UWUSpeedometer",
                help: "Enables or disables the UWU.Speedometer option",
                adminOnly: false,
                isCheat: false,
                () => EnableSpeedometer.Value,
                (value) => EnableSpeedometer.Value = value
            ));
        }

        internal static void Patch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Player), nameof(Player.CustomFixedUpdate));
            var postfix = AccessTools.Method(typeof(SpeedometerFeature), nameof(SpeedometerFeature.Player_CustomFixedUpdate_PostFix));
            harmony.Patch(original, postfix: new(postfix));
        }

        internal static void OnGUI()
        {
            // Only draw the GUI if the player exists and the game HUD is visible.
            if (Player.m_localPlayer == null || !EnableSpeedometer.Value)
            {
                return;
            }

            // Define the style for our label's text.
            GUIStyle style = new()
            {
                fontSize = 32, // A good readable size
                alignment = TextAnchor.MiddleCenter,
            };
            style.normal.textColor = Color.white; // White text

            // We also change the unit depending on whether the player is on a ship or not.
            string speedText = $"{Mathf.Floor(currentSpeed)} m/s";

            // This will place it 10 pixels from the top and 10 from the left.
            Rect labelRect = new(10, 10, 144, 40);

            // Draw a semi-transparent background for better readability
            Color backgroundColor = new(0, 0, 0, 0.5f); // Black with 50% opacity
            GUI.backgroundColor = backgroundColor;
            GUI.Box(labelRect, GUIContent.none);

            // Draw the actual label with the speed text.
            GUI.Label(labelRect, speedText, style);
        }

        static void Player_CustomFixedUpdate_PostFix(Player __instance, float fixedDeltaTime)
        {
            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            // Check every 1/2 second
            updateTimer += Time.deltaTime;
            if (updateTimer < maxTime) return;
            updateTimer = 0f;

            currentSpeed = __instance.GetVelocity().magnitude;
        }
    }

}