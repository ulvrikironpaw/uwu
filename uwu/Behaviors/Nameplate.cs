using System.Linq;
using TMPro;
using UnityEngine;
using UWU.Common;

namespace UWU.Behaviors
{
  /// <summary>
  /// Adds a billboard to the parent object.
  /// </summary>
  internal class Nameplate : MonoBehaviour
  {
    // Must be set after AddComponent in the parent.
    internal MonoBehaviour target;
    private TextMeshPro textMeshPro;

    void Start()
    {
      // Create text mesh.
      textMeshPro = gameObject.AddComponent<TextMeshPro>();
      textMeshPro.font = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
          .FirstOrDefault(f => f.name == "Valheim-Norse");
      textMeshPro.fontStyle = FontStyles.Bold;
      textMeshPro.fontSize = 10f;
      // White.
      textMeshPro.color = new Color(1f, 1f, 1f);
      textMeshPro.alignment = TextAlignmentOptions.Center;
      textMeshPro.enableAutoSizing = false;

      var mat = new Material(textMeshPro.font.material);
      mat.EnableKeyword("OUTLINE_ON");
      mat.SetColor("_OutlineColor", new Color(0f, 0f, 0.3f, 1f)); // Dark blue
      mat.SetFloat("_OutlineWidth", 0.085f);
      textMeshPro.fontMaterial = mat;
    }

    void LateUpdate()
    {
      if (Camera.main == null || target == null || textMeshPro == null) return;

      // Face the camera
      var lookDirection = transform.position - Camera.main.transform.position;
      // Make the nameplate stay up and down.
      lookDirection.y = 0;

      // Avoid a 0ish value for look direction.
      if (lookDirection.sqrMagnitude > 0.001f)
      {
        transform.rotation = Quaternion.LookRotation(lookDirection);
      }

      // Update text
      string name = ObjectUtils.GetLabelFromObject(target);
      textMeshPro.text = string.IsNullOrWhiteSpace(name) ? "" : name.Trim();
    }

    internal static void RemoveNameplates(MonoBehaviour target)
    {
      var billboard = target.gameObject.GetComponent<Nameplate>();
      if (billboard != null)
      {
        Destroy(billboard);
      }
    }

    internal static void DecorateIfNecessary(MonoBehaviour target)
    {
      // Skip if already has a label
      if (target.GetComponentInChildren<Nameplate>() == null)
      {
        Nameplate.Decorate(target);
      }
    }

    internal static void Decorate(MonoBehaviour target)
    {
      Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
      if (renderers.Length < 0) return;

      var combined = renderers[0].bounds;
      foreach (var r in renderers)
      {
        combined.Encapsulate(r.bounds);
      }

      // Top point in world space
      Vector3 topWorld = new(
          combined.center.x,
          combined.max.y,
          combined.center.z
      );

      // Convert to ship local space
      Vector3 topLocal = target.transform.InverseTransformPoint(topWorld);

      // Create label 
      var go = new GameObject();
      go.transform.SetParent(target.transform);
      go.transform.localPosition = topLocal + (Vector3.up * 0.5f) + (Vector3.forward * 0.5f);
      go.transform.localRotation = Quaternion.identity;
      go.transform.localScale = Vector3.one;

      var nameplate = go.AddComponent<Nameplate>();
      nameplate.target = target;
    }
  }
}
