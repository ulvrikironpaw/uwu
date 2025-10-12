using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWU.Common;

namespace UWU.Features
{
  internal sealed class ShipPinFeature : UWUFeature
  {
    internal override string Name => "ShipPin";
    protected override string Category => "Sailing";
    protected override string Description => "Tracks ships on the map";

    private const float scanInterval = 5f;
    private const float updateInterval = 0.5f;
    private readonly Dictionary<ZDO, Minimap.PinData> SailPins = new();
    private float scanTimer = 0f;
    private float updateTimer = 0f;

    protected override void OnUpdate()
    {
      if (Minimap.instance == null) return;

      scanTimer += Time.deltaTime;
      if (scanTimer >= scanInterval)
      {
        scanTimer = 0f;
        RescanShips();
      }

      updateTimer += Time.deltaTime;
      // 1 second interval
      if (updateTimer >= updateInterval)
      {
        updateTimer = 0f;
        UpdateSailPins();
      }
    }

    protected override void OnUnpatch()
    {
      RemoveSailPins();
    }

    private void RemoveSailPins()
    {
      foreach (var value in SailPins.Values)
      {
        Minimap.instance.RemovePin(value);
      }
      SailPins.Clear();
    }

    private void RescanShips()
    {
      var allShips = ObjectUtils.EnumerateZDOsOfType<Ship>().ToList();

      // Add all ships that are new from the scan.
      foreach (ZDO ship in allShips)
      {
        if (!SailPins.ContainsKey(ship))
        {
          // Add a pin for new ships
          var displayName = ObjectUtils.GetLabelFromZDO(ship);
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

    private void UpdateSailPins()
    {
      // Find all ships and update/add pins
      foreach (var kvp in SailPins)
      {
        // Update the name
        kvp.Value.m_name = ObjectUtils.GetLabelFromZDO(kvp.Key);
        // Update pin position
        kvp.Value.m_pos = kvp.Key.GetPosition();
      }
    }
  }
}
