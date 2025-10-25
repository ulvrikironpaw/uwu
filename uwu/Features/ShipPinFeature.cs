using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWU.Behaviors;
using UWU.Common;

namespace UWU.Features
{
  internal sealed class ShipPinFeature : FeatureBehaviour
  {
    private const float ICON_SCALE = 1.35f;
    protected override string Name => "ShipPin";
    protected override string Category => "Sailing";
    protected override string Description => "Tracks ships on the map";

    private const float scanInterval = 5f;
    private const float updateInterval = 0.17f;
    private readonly Dictionary<ZDO, SailPinData> SailPins = new();
    private float scanTimer = 0f;
    private float updateTimer = 0f;

    void FixedUpdate()
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
      foreach (SailPinData value in SailPins.Values)
      {
        Minimap.instance.RemovePin(value.PinData);
      }
      SailPins.Clear();
    }

    private void RescanShips()
    {
      var allShips = ObjectUtils.EnumerateZDOsOfTypeByPosition<Ship>().ToList();

      // Add all ships that are new from the scan.
      foreach (ZDO ship in allShips)
      {
        var isInitialSetup = !SailPins.ContainsKey(ship);
        if (isInitialSetup)
        {
          // Add a pin for new ships
          var buildIcon = IconUtils.GetBuildIconFromZDO(ship);
          var displayName = NameCache.GetLabelFromZDO(ship);
          var pinData = Minimap.instance.AddPin(
              ship.GetPosition(),
              buildIcon == null ? Minimap.PinType.Icon3 : Minimap.PinType.None,
              displayName,
              save: false,
              isChecked: false
          );
          if (buildIcon != null)
          {
            pinData.m_icon = buildIcon;
          }
          SailPins[ship] = new SailPinData { PinData = pinData };
        }
      }
      // Clean up ships that no longer exist.
      foreach (var kvp in SailPins.ToList())
      {
        var ship = kvp.Key;
        if (!allShips.Contains(ship))
        {
          SailPinData pinData = kvp.Value;
          Minimap.instance.RemovePin(pinData.PinData);
          SailPins.Remove(ship);
        }
      }
    }

    private void UpdateSailPins()
    {
      // Find all ships and update/add pins
      foreach (var kvp in SailPins)
      {
        var ship = kvp.Key;
        var sailPinData = kvp.Value;

        if (sailPinData.IsIconSetup)
        {
          // Try to flip the icon when the ship goes east.
          var image = sailPinData.PinData?.m_iconElement;
          if (image != null)
          {
            var rt = image.rectTransform;
            var scale = rt.localScale;
            scale.x = ICON_SCALE;
            scale.y = ICON_SCALE;
            rt.localScale = scale;
            sailPinData.IsIconSetup = true;
          }
        }

        // Update the name
        sailPinData.PinData.m_name = NameCache.GetLabelFromZDO(ship);
        // Update pin position
        sailPinData.PinData.m_pos = ship.GetPosition();
      }
    }

    class SailPinData
    {
      internal Minimap.PinData PinData { get; set; }

      internal bool IsIconSetup { get; set; } = true;
    }
  }
}
