using UnityEngine;
using UWU.Behaviors;
using UWU.Common;

namespace UWU.Features
{
  internal class ShipNameplateFeature : FeatureBehaviour
  {
    protected override string Name => "ShipNameplates";
    protected override string Category => "Sailing";
    protected override string Description => "Adds nameplates to boats";
    protected override bool EnabledByDefault => true;

    private const float scanInterval = 5f;
    private float scanTimer = 5f;

    void FixedUpdate()
    {
      scanTimer += Time.deltaTime;
      if (scanTimer < scanInterval) return;
      scanTimer = 0f;

      // list of all ships that are loaded in the world for this player.
      foreach (var ship in ObjectUtils.EmumerateInstanceOfType<Ship>())
      {
        Nameplate.DecorateIfNecessary(ship);
      }
    }

    protected override void OnUnpatch()
    {
      // Remove nameplates on ships if they exist.
      foreach (var ship in ObjectUtils.EmumerateInstanceOfType<Ship>())
      {
        Nameplate.RemoveNameplates(ship);
      }
      // Set the timer to max to force a rescan immediately on enablement.
      scanTimer = scanInterval;
    }
  }
}