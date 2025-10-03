using BepInEx.Configuration;
using Jotunn.Extensions;
using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace UWU
{
    internal class SailPinFeature
    {
        private static ConfigEntry<bool> EnableSailPins;
        private static readonly Dictionary<Ship, Minimap.PinData> SailPins = new();
        private static float updateTimer = 0f;

        internal static void Configure(ConfigFile config)
        {
            EnableSailPins = config.BindConfig(
                section: "Sailing",
                key: "SailPin",
                defaultValue: true,
                description: "Places pins on the map for active sail boats",
                synced: false
            );

            CommandManager.Instance.AddConsoleCommand(new BoolConsoleCommand(
                name: "UWUSailPinFeature",
                help: "Enables or disables the UWU.SailPin option",
                adminOnly: true,
                isCheat: false,
                (value) => EnableSailPins.Value = value
            ));


        }
        internal static void Update()
        {
            if (!EnableSailPins.Value)
            {
                RemoveSailPins();
                return;
            }

            if (Minimap.instance == null)
            {
                return;
            }

            updateTimer += Time.deltaTime;
            // 1 second interval
            if (updateTimer >= 1f)
            {
                updateTimer = 0f;
                UpdateSailPins();
            }
        }

        private static void RemoveSailPins()
        {
            SailPins.Clear();
        }

        private static void UpdateSailPins()
        {
            // Find all ships and update/add pins
            foreach (var ship in Object.FindObjectsByType<Ship>(FindObjectsSortMode.InstanceID))
            {
                if (!SailPins.TryGetValue(ship, out var pin))
                {
                    // Add a pin for new ships
                    // TODO: localize "Clone" or find building piece name.
                    pin = Minimap.instance.AddPin(
                        ship.transform.position,
                        Minimap.PinType.Icon3,
                        $"{ship.name}".Replace("(Clone)", ""),
                        save: false,
                        isChecked: false
                    );
                    SailPins[ship] = pin;
                }

                // Update pin position
                pin.m_pos = ship.transform.position;
            }

            // Clean up destroyed ships
            var destroyed = new List<Ship>();
            foreach (var kvp in SailPins)
            {
                if (kvp.Key == null || !kvp.Key.gameObject)
                {
                    Minimap.instance.RemovePin(kvp.Value);
                    destroyed.Add(kvp.Key);
                }
            }
            foreach (var ship in destroyed)
            {
                SailPins.Remove(ship);
            }
        }

    }
}
