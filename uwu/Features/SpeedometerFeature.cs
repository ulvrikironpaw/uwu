using UnityEngine;
using UWU.Common;

namespace UWU.Features
{
    internal sealed class SpeedometerFeature : UWUFeature
    {
        internal override string Name => "Speedometer";
        protected override string Category => "Sailing";
        protected override string Description => "If enabled, a speedometer will appear on the screen";

        private const float maxTime = 0.25f;

        // This static variable will hold our calculated speed.
        // We make it static so the patch can set it, and the GUI method can read it.
        private float currentSpeed = 0f;
        private float updateTimer = 0f;

        protected override void OnGUI()
        {
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

        protected override void OnUpdate()
        {
            var localPlayer = Player.m_localPlayer;
            if (localPlayer == null) return;

            // Check every 1/2 second
            updateTimer += Time.deltaTime;
            if (updateTimer < maxTime) return;
            updateTimer = 0f;

            currentSpeed = localPlayer.GetVelocity().magnitude;
        }
    }
}