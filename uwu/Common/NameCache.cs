using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace UWU.Common
{
  internal class NameCache
  {
    private static readonly Dictionary<int, string> nameCache = new();

    static NameCache()
    {
      // Register building the cache onces both vanilla
      // and custom prefabs are registered. 
      PrefabManager.OnPrefabsRegistered += () =>
      {
        nameCache.Clear();
      };
    }

    /// <summary>
    /// Gets the prefab name for a given prefab hash (if cached).
    /// </summary>
    internal static string GetPrefabName(int prefabHash)
    {
      if (nameCache.TryGetValue(prefabHash, out string name))
      {
        return name;
      }
      name = ZNetScene.instance?.GetPrefab(prefabHash)?.name ?? "";
      if (name == "")
      {
        return "(unknown prefab)";
      }
      nameCache.Add(prefabHash, name);
      return name;
    }

    /// <summary>
    /// Gets a display label (friendly name) for the prefab hash.
    /// </summary>
    internal static string GetLabel(int prefabHash)
    {
      return ApplyNameOverrides(GetPrefabName(prefabHash));
    }


    /// <summary>
    /// Returns the display name.
    /// </summary>
    internal static string GetLabelFromObject(MonoBehaviour behavior)
    {
      var znetView = ObjectUtils.GetZNetView(behavior);
      if (znetView == null) return "(unknown ZNetView)";

      return GetLabelFromZNetView(znetView);
    }

    /// <summary>
    /// Returns the display name.
    /// </summary>
    internal static string GetLabelFromZNetView(ZNetView netView)
    {
      var zdo = netView.GetZDO();
      if (zdo == null) return "(unknown ZDO)";

      return GetLabelFromZDO(zdo);
    }

    /// <summary>
    /// Returns the display name.
    /// </summary>
    internal static string GetLabelFromZDO(ZDO zdo)
    {
      var uwuName = GetCustomLabelFromZDO(zdo);
      if (!string.IsNullOrWhiteSpace(uwuName)) return uwuName;

      var prefab = zdo.GetPrefab();
      if (prefab == 0) return "(unknown Prefab)";

      return GetLabel(prefab);
    }

    /// <summary>
    /// Returns the name.
    /// </summary>
    internal static string GetNameFromZDO(ZDO zdo)
    {
      return GetPrefabName(zdo.GetPrefab());
    }

    /// <summary>
    /// Returns the display name.
    /// </summary>
    internal static string GetCustomLabelFromZDO(ZDO zdo)
        => zdo.GetString(CustomProperties.CUSTOM_LABEL_PROPERTY, "") ?? "";

    /// <summary>
    /// Returns the display name.
    /// </summary>
    private static string ApplyNameOverrides(string prefabName)
    {
      if (string.IsNullOrWhiteSpace(prefabName))
      {
        return "(unknown object name)";
      }

      return prefabName.ToLowerInvariant() switch
      {
        "vikingship" => "Longship",
        "vikingship_ashlands" => "Drakkar",
        _ => prefabName,
      };
    }
  }
}
