using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Extensions;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UWU
{
    internal class ShipPinFeature
    {
        private static ConfigEntry<bool> EnableShipPin;
        private static readonly Dictionary<ZDO, Minimap.PinData> SailPins = new();
        private const float scanInterval = 5f;
        private static float scanTimer = 0f;
        private const float updateInterval = 0.5f;
        private static float updateTimer = 0f;

        internal static void Configure(ConfigFile config)
        {
            EnableShipPin = config.BindConfig(
                section: "Sailing",
                key: "ShipPin",
                defaultValue: true,
                description: "Tracks ships on the map",
                synced: true
            );

            CommandManager.Instance.AddConsoleCommand(new BoolConsoleCommand(
                name: "UWUShipPin",
                help: "Enables or disables the UWU.ShipPin option",
                adminOnly: true,
                isCheat: false,
                () => EnableShipPin.Value,
                (value) => EnableShipPin.Value = value
            ));


        }
        internal static void Update()
        {
            if (!EnableShipPin.Value)
            {
                RemoveSailPins();
                return;
            }

            if (Minimap.instance == null)
            {
                return;
            }

            scanTimer += Time.deltaTime;
            if (scanTimer >= scanInterval)
            {
                scanTimer = 0f;
                ScanShips();
            }

            updateTimer += Time.deltaTime;
            // 1 second interval
            if (updateTimer >= updateInterval)
            {
                updateTimer = 0f;
                UpdateSailPins();
            }
        }

        private static void RemoveSailPins()
        {
            foreach (var value in SailPins.Values)
            {
                Minimap.instance.RemovePin(value);
            }
            SailPins.Clear();
        }

        private static void ScanShips()
        {
            var allShips = GetAllShips();
            // Add all ships that are new from the scan.
            foreach (ZDO ship in allShips)
            {
                if (!SailPins.ContainsKey(ship))
                {
                    // Add a pin for new ships
                    var displayName = GetShipDisplayName(ship);
                    var pin = Minimap.instance.AddPin(
                        ship.GetPosition(),
                        Minimap.PinType.Icon3,
                        displayName,
                        save: false,
                        isChecked: false
                    );
                    SailPins[ship] = pin;
                }
            }
            // Clean up ships that no longer exist.
            foreach (var kvp in SailPins.ToList())
            {
                var ship = kvp.Key;
                if (!allShips.Contains(ship))
                {
                    var pin = kvp.Value;
                    Minimap.instance.RemovePin(pin);
                    SailPins.Remove(ship);
                }
            }
        }

        private static void UpdateSailPins()
        {
            // Find all ships and update/add pins
            foreach (var kvp in SailPins)
            {
                // Update pin position
                kvp.Value.m_pos = kvp.Key.GetPosition();
            }
        }

        private static List<ZDO> GetAllShips()
        {
            var objects = Traverse.Create(ZDOMan.instance).Field("m_objectsByID").GetValue() as Dictionary<ZDOID, ZDO>;
            if (objects == null)
            {
                return new();
            }

            var destroyedList = Traverse.Create(ZDOMan.instance).Field("m_destroySendList").GetValue() as List<ZDOID>;
            var ships = new List<ZDO>();
            foreach (var zdo in objects.Values)
            {
                var displayName = GetShipDisplayName(zdo);
                if (displayName == "Karve" ||
                    displayName == "Raft" ||
                    displayName == "Longship" ||
                    displayName == "Drakkar")
                {
                    if (zdo.GetPrefab() != 0 && (destroyedList == null || !destroyedList.Contains(zdo.m_uid)))
                    {
                        ships.Add(zdo);
                    }
                }
            }
            return ships;
        }

        private static string GetShipDisplayName(ZDO zdo)
        {
            int prefab = zdo.GetPrefab();
            string prefabName = ZNetScene.instance.GetPrefab(prefab)?.name;
            return prefabName.ToLower() switch
            {
                "vikingship" => "Longship",
                "shipdrakkar" => "Drakkar",
                _ => prefabName,
            };
        }
    }
}
