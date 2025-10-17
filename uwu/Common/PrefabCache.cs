using Jotunn.Managers;
using System.Collections.Generic;

namespace UWU.Common
{
  /// <summary>
  /// Keeps a running cache for which prefabs have Component of T. It is
  /// worth noting that each 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  internal static class PrefabCache<T>
  {
    private static HashSet<int> hashcodeCache = new();
    private static Dictionary<int, string> nameCache = new();
    private static Dictionary<int, string> labelCache = new();
    private static bool isBuilt = false;

    static PrefabCache()
    {
      // Register building the cache onces both vanilla
      // and custom prefabs are registered. 
      PrefabManager.OnPrefabsRegistered += () => Build(true);
    }

    /// <summary>
    /// True if the cache was built.
    /// </summary>
    /// <param name="forceRebuild"></param>
    /// <returns></returns>
    public static bool Build(bool forceRebuild = false)
    {
      if (isBuilt && !forceRebuild) return true;

      var scene = ZNetScene.instance;
      if (scene == null) return false;

      var newHashcodeCache = new HashSet<int>();
      var newNameCache = new Dictionary<int, string>();
      var newLabelCache = new Dictionary<int, string>();
      foreach (var prefab in scene.m_prefabs)
      {
        if (prefab == null) continue;
        if (prefab.GetComponent<T>() != null)
        {
          var hash = prefab.name.GetStableHashCode();
          newHashcodeCache.Add(hash);
          newNameCache[hash] = prefab.name;
          newLabelCache[hash] = RsolvePrefabLabel(prefab.name);
        }
      }

      isBuilt = true;
      hashcodeCache = newHashcodeCache;
      nameCache = newNameCache;
      labelCache = newLabelCache;
      return true;
    }

    /// <summary>
    /// True if the prefab has this type within it.
    /// </summary>
    /// <param name="prefabHash"></param>
    /// <returns></returns>
    internal static bool Contains(int prefabHash)
    {
      if (!isBuilt)
      {
        Build();
      }
      return hashcodeCache.Contains(prefabHash);
    }


    /// <summary>
    /// Gets the prefab name for a given prefab hash (if cached).
    /// </summary>
    internal static string GetPrefabName(int prefabHash)
    {
      if (!isBuilt)
      {
        Build();
      }

      if (nameCache.TryGetValue(prefabHash, out var name)) return name;
      return "(unknown prefab)";
    }

    /// <summary>
    /// Gets a display label (friendly name) for the prefab hash.
    /// </summary>
    internal static string GetLabel(int prefabHash)
    {
      if (!isBuilt)
      {
        Build();
      }

      if (labelCache.TryGetValue(prefabHash, out var label)) return label;
      return "(unknown prefab)";
    }


    /// <summary>
    /// Returns the display name.
    /// </summary>
    private static string RsolvePrefabLabel(string prefabName)
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
