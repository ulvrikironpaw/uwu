using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWU.Extensions;

namespace UWU.Common
{
  internal class ObjectUtils
  {

    /// <summary>
    /// Gets all saved instances of a specific type of component. Avoid using in a tight loop.
    /// </summary>
    internal static IEnumerable<ZDO> EnumerateZDOsOfType<T>() where T : MonoBehaviour
    {
      PrefabCache<T>.Build();

      var zdoman = ZDOMan.instance;
      if (zdoman == null)
        yield break;

      var objects = zdoman.GetObjectsById_UWU();
      if (objects == null || objects.Count == 0)
        yield break;

      var destroyedList = zdoman.GetDestroyedSendList_UWU();
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

      var instances = scene.GetInstances_UWU();
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

      var instances = scene.GetInstances_UWU();
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