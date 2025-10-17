using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UWU.Common
{
  internal class ObjectUtils
  {
    private static readonly AccessTools.FieldRef<ZDOMan, Dictionary<ZDOID, ZDO>> ObjectsByIdRef =
      AccessTools.FieldRefAccess<ZDOMan, Dictionary<ZDOID, ZDO>>("m_objectsByID");

    private static readonly AccessTools.FieldRef<ZDOMan, List<ZDOID>> DestroySendListRef =
      AccessTools.FieldRefAccess<ZDOMan, List<ZDOID>>("m_destroySendList");

    private static readonly AccessTools.FieldRef<ZNetScene, Dictionary<ZDO, ZNetView>> Instances =
      AccessTools.FieldRefAccess<ZNetScene, Dictionary<ZDO, ZNetView>>("m_instances");

    /// <summary>
    /// Gets all saved instances of a specific type of component. Avoid using in a tight loop.
    /// </summary>
    internal static IEnumerable<ZDO> EnumerateZDOsOfType<T>() where T : MonoBehaviour
    {
      PrefabCache<T>.Build();

      var zdoman = ZDOMan.instance;
      if (zdoman == null)
        yield break;

      var objects = ObjectsByIdRef(zdoman);
      if (objects == null || objects.Count == 0)
        yield break;

      var destroyedList = DestroySendListRef(zdoman);
      var destroyedSet = destroyedList == null || destroyedList.Count == 0
        ? null
        : new HashSet<ZDOID>(destroyedList);
      foreach (var zdo in objects.Values)
      {
        var prefabHash = zdo.GetPrefab();
        if (prefabHash != 0
          && PrefabCache<T>.Contains(prefabHash)
          && (destroyedSet == null || !destroyedSet.Contains(zdo.m_uid)))
        {
          yield return zdo;
        }
      }
    }

    /// <summary>
    /// Same as EnumerateZDOsOfType but presorts by z and then x position.
    /// </summary>
    internal static IEnumerable<ZDO> EnumerateZDOsOfTypeByPosition<T>() where T : MonoBehaviour
    {
      return EnumerateZDOsOfType<T>()
        .OrderByDescending(zdo => zdo.GetPosition().z)
        .ThenBy(zdo => zdo.GetPosition().x);
    }

    /// <summary>
    /// Enumerates all ZDOs with that type included that are actively loaded.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal static IEnumerable<ZDO> EnumerateLoadedZDOsOfType<T>() where T : MonoBehaviour
    {
      PrefabCache<T>.Build();

      var scene = ZNetScene.instance;
      if (scene == null) yield break;

      var instances = Instances(scene);
      foreach (var view in instances.Values)
      {
        if (view == null) continue;

        var zdo = view.GetZDO();
        if (zdo == null) continue;

        if (!PrefabCache<T>.Contains(zdo.GetPrefab())) continue;

        yield return zdo;
      }
    }

    /// <summary>
    /// Enumerates all T Components that are currently loaded.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal static IEnumerable<T> EmumerateInstanceOfType<T>() where T : MonoBehaviour
    {
      PrefabCache<T>.Build();

      var scene = ZNetScene.instance;
      if (scene == null) yield break;

      var instances = Instances(scene);
      foreach (var view in instances.Values)
      {
        if (view == null) continue;

        var zdo = view.GetZDO();
        if (zdo == null) continue;

        if (!PrefabCache<T>.Contains(zdo.GetPrefab())) continue;

        var component = view.GetComponent<T>();
        if (component != null)
          yield return component;
      }
    }

    /// <summary>
    /// Returns the display name.
    /// </summary>
    internal static ZNetView GetZNetView(MonoBehaviour behavior)
    {
      return behavior.gameObject.GetComponent<ZNetView>();
    }
  }
}