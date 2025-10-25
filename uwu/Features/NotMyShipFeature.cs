using HarmonyLib;
using UWU.Behaviors;

namespace UWU.Features
{
  internal sealed class NotMyShipFeature : FeatureBehaviour
  {
    protected override string Name => "NotMyShip";
    protected override string Category => "Sailing";
    protected override string Description => "Aggression toward ships will be reduced while no player is aboard";

    protected override void OnPatch(Harmony harmony)
    {
      var original = AccessTools.Method(
        typeof(BaseAI),
        nameof(BaseAI.CanSenseTarget),
        new[] { typeof(Character) });
      var postfix = AccessTools.Method(
        typeof(NotMyShipFeature),
        nameof(BaseAI_CanSenseTarget_Postfix));
      harmony.Patch(original, postfix: new(postfix));
    }

    private static void BaseAI_CanSenseTarget_Postfix(Character target, ref bool __result)
    {
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
