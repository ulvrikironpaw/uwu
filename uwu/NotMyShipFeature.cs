using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Extensions;
using Jotunn.Managers;

namespace UWU
{
    internal class NotMyShipFeature
    {

        private static ConfigEntry<bool> EnableNotMyShip;

        internal static void Configure(ConfigFile config)
        {
            EnableNotMyShip = config.BindConfig(
                section: "Sailing",
                key: "NotMyShip",
                defaultValue: true,
                description: "If enabled, aggression toward ships will be reduced while no player is aboard.",
                synced: true
            );

            CommandManager.Instance.AddConsoleCommand(new BoolConsoleCommand(
                name: "UWUNotMyShip",
                help: "Enables or disables the UWU.NotMyShip option",
                adminOnly: true,
                isCheat: false,
                () => EnableNotMyShip.Value,
                (value) => EnableNotMyShip.Value = value
            ));
        }

        internal static void Patch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(BaseAI), nameof(BaseAI.CanSenseTarget), new[] { typeof(Character) });
            var postfix = AccessTools.Method(typeof(NotMyShipFeature), nameof(BaseAI_CanSenseTarget_Postfix));
            harmony.Patch(original, postfix: new(postfix));
        }

        private static void BaseAI_CanSenseTarget_Postfix(Character target, ref bool __result)
        {
            if (!EnableNotMyShip.Value) return;

            // Don't turn a false verdict into a true.
            // Don't return true if there is no target.
            if (!__result || target == null) return;
            // Return if the target isn't a ship.
            var ship = target.GetComponent<Ship>();
            if (ship == null) return;
            // Only let the BaseAI see the ship if it has a player on board.
            __result = ship.HasPlayerOnboard();
        }
    }
}
