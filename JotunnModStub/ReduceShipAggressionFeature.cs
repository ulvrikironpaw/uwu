using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Extensions;
using Jotunn.Managers;

namespace UWU
{
    internal class ReduceShipAggressionFeature
    {

        private static ConfigEntry<bool> EnableReduceShipAggression;

        internal static void Configure(ConfigFile config)
        {
            EnableReduceShipAggression = config.BindConfig(
                section: "Sailing",
                key: "ReduceShipAggression",
                defaultValue: true,
                description: "If enabled, aggression toward ships will be reduces while no one is aboard.",
                synced: true
            );

            CommandManager.Instance.AddConsoleCommand(new BoolConsoleCommand(
                name: "UWUReduceShipAggression",
                help: "Enables or disables the UWU.EnableReduceSailboatAggression option",
                adminOnly: true,
                isCheat: false,
                () => EnableReduceShipAggression.Value,
                (value) => EnableReduceShipAggression.Value = value
            ));
        }

        internal static void Patch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(BaseAI), nameof(BaseAI.CanSenseTarget), new[] { typeof(Character) });
            var postfix = AccessTools.Method(typeof(ReduceShipAggressionFeature), nameof(BaseAI_CanSenseTarget_Postfix));
            harmony.Patch(original, postfix: new(postfix));
        }

        private static void BaseAI_CanSenseTarget_Postfix(Character target, ref bool __result)
        {
            if (!EnableReduceShipAggression.Value) return;

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
