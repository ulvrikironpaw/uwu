using UnityEngine;

namespace UWU.Common
{
  internal static class PhysicsUtils
  {
    /// <summary>
    /// Gets the top center point of the given transform.
    /// </summary>
    /// <param name="attachTransform"></param>
    /// <returns></returns>
    internal static Vector3 GetTopCenterPoint(Transform attachTransform)
    {
      Renderer[] renderers = attachTransform.GetComponentsInChildren<Renderer>();

      if (renderers.Length == 0) return attachTransform.position;

      Bounds bounds = renderers[0].bounds;
      for (int i = 1; i < renderers.Length; i++)
      {
        bounds.Encapsulate(renderers[i].bounds);
      }

      return new Vector3(
          bounds.center.x,
          bounds.max.y,
          bounds.center.z
      );
    }

#if DEBUG
    internal static void PrintAllChildTransforms(Transform root, string indent = "")
    {
      Debug.Log($"{indent}{root.name}");
      foreach (Transform child in root)
      {
        PrintAllChildTransforms(child, indent + "  ");
      }
    }
#endif
  }
}