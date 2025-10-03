using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Extensions;
using Jotunn.Managers;
using UnityEngine;

namespace UWU
{
    static class ShipBonkiesFeature
    {
        private static ConfigEntry<bool> EnableShipBonkies;

        internal static void Configure(ConfigFile config)
        {
            EnableShipBonkies = config.BindConfig(
                section: "Sailing",
                key: "ShipBonkies",
                defaultValue: true,
                description: "If enabled, Hammer destructs ships for a full refund when no player is aboard",
                synced: true
            );

            CommandManager.Instance.AddConsoleCommand(new BoolConsoleCommand(
                name: "UWUShipBonkies",
                help: "Enables or disables the UWU.ShipBonkies option",
                adminOnly: true,
                isCheat: false,
                () => EnableShipBonkies.Value,
                (value) => EnableShipBonkies.Value = value
            ));
        }

        internal static void Patch(Harmony harmony)
        {
            PatchPlayerRemovePiece(harmony);
        }

        private static void PatchPlayerRemovePiece(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Player), "RemovePiece");
            var prefix = AccessTools.Method(typeof(ShipBonkiesFeature), nameof(Player_RemovePiece_Prefix));
            harmony.Patch(original, prefix: new(prefix));
        }

        static bool Player_RemovePiece_Prefix(ref bool __result)
        {
            if (!EnableShipBonkies.Value) return true;

            var localPlayer = Player.m_localPlayer;
            if (localPlayer == null) return true;

            // Only raycast vehicles and character triggers. This keeps the raycast
            // from accidentally finding water volume, etc.
            var layerMask = (1 << (int)ValheimLayer.CharacterTrigger) | (1 << (int)ValheimLayer.Vehicle);
            var hasHit = Physics.Raycast(
                GameCamera.instance.transform.position,
                GameCamera.instance.transform.forward,
                out RaycastHit raycastHit,
                10f,
                layerMask
            );
            // Return to the original method if no target was found.
            if (!hasHit) return true;
            // Check if the hit object has a Ship component in its hierarchy
            Ship ship = raycastHit.collider.GetComponentInParent<Ship>();
            if (ship == null) return true;

            if (ship.HasPlayerOnboard())
            {
                // This is an indication we know this shouldn't be removed even by vanilla logic.
                // Short circuit.
                MessageHud.instance?.ShowMessage(MessageHud.MessageType.Center, "Exit boat and try again");
                __result = false;
                return false;
            }

            var wearNTear = ship.GetComponent<WearNTear>();
            if (wearNTear == null)
            {
                // Get the WearNTear component that handles damage.
                // This should never be null, short circuit to prevent issues.
                __result = false;
                return false;
            }

            wearNTear.Remove();
            __result = true;
            return false;
        }

    }
}
