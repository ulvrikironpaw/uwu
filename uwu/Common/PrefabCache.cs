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
    internal static bool Build(bool forceRebuild = false)
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
        }
      }

      isBuilt = true;
      hashcodeCache = newHashcodeCache;
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

  }
}
