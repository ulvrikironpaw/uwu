using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UWU.Common
{
    internal class ObjectUtils
    {
        /// <summary>
        /// Gets all saved instances of a specific type of component. Avoid using in a tight loop.
        /// </summary>
        internal static IEnumerable<ZDO> EnumerateZDOsOfType<T>() where T : MonoBehaviour
        {
            var objects = Traverse.Create(ZDOMan.instance).Field("m_objectsByID").GetValue() as Dictionary<ZDOID, ZDO>;
            if (objects == null) return new List<ZDO>();

            var destroyedList = Traverse.Create(ZDOMan.instance).Field("m_destroySendList").GetValue() as List<ZDOID>;
            return objects.Values.Where(zdo =>
            {
                int prefabHash = zdo.GetPrefab();
                if (prefabHash == 0) return false;

                var prefab = ZNetScene.instance.GetPrefab(prefabHash);
                if (prefab == null || !prefab.GetComponents<T>().Any()) return false;

                return true;
            });
        }

        /// <summary>
        /// Gets all instances alive (in the current client session).
        /// </summary>
        internal static IEnumerable<T> EmumerateInstanceOfType<T>() where T : MonoBehaviour
        {
            return Object.FindObjectsByType(typeof(T), FindObjectsSortMode.None)
                .AsEnumerable()
                .Cast<T>();
        }

        /// <summary>
        /// Returns the display name.
        /// </summary>
        internal static ZNetView GetZNetView(MonoBehaviour behavior)
        {
            return behavior.gameObject.GetComponent<ZNetView>();
        }

        /// <summary>
        /// Returns the display name.
        /// </summary>
        internal static string GetLabelFromObject(MonoBehaviour behavior)
        {
            var znetView = GetZNetView(behavior);
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

            string prefabName = ZNetScene.instance.GetPrefab(prefab)?.name ?? "";
            return GetLabelFromPrefab(prefabName);
        }

        /// <summary>
        /// Returns the display name.
        /// </summary>
        internal static string GetCustomLabelFromZDO(ZDO zdo)
            => zdo.GetString(Constants.CUSTOM_LABEL_PROPERTY, "") ?? "";

        /// <summary>
        /// Returns the display name.
        /// </summary>
        internal static string GetLabelFromPrefab(string prefabName)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return "(unknown object name)";
            }

            return prefabName.ToLower() switch
            {
                "vikingship" => "Longship",
                "vikingship_ashlands" => "Drakkar",
                _ => prefabName,
            };
        }
    }
}
