using HarmonyLib;
using UnityEngine;
using UWU.Common;

namespace UWU.Features
{
    internal sealed class ShipBonkiesFeature : UWUFeature
    {
        private static ShipBonkiesFeature instance;

        internal override string Name => "ShipBonkies";
        protected override string Category => "Sailing";
        protected override string Description => "Hammer destructs ships for full refund when no player is aboard";

        internal ShipBonkiesFeature()
        {
            instance = this;
        }

        protected override void OnPatch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Player), "RemovePiece");
            var prefix = AccessTools.Method(typeof(ShipBonkiesFeature), nameof(Player_RemovePiece_Prefix));
            harmony.Patch(original, prefix: new(prefix));
        }

        private static bool Player_RemovePiece_Prefix(ref bool __result)
        {
            if (!instance.Enabled.Value) return true;

            var localPlayer = Player.m_localPlayer;
            if (localPlayer == null) return true;

            // Only raycast vehicles and character triggers. This keeps the raycast
            // from accidentally finding water volume, etc.
            var layerMask = 1 << (int)ValheimLayer.CharacterTrigger | 1 << (int)ValheimLayer.Vehicle;
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
