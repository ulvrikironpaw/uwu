using System.Linq;
using TMPro;
using UnityEngine;
using UWU.Common;

namespace UWU.Features
{
  internal class ShipNameplateFeature : UWUFeature
  {
    internal override string Name => "ShipNameplates";
    protected override string Category => "Sailing";
    protected override string Description => "Adds nameplates to boats";
    protected override bool EnabledByDefault => true;

    private const float scanInterval = 5f;
    private float scanTimer = 5f;

    protected override void OnUpdate()
    {
      scanTimer += Time.deltaTime;
      if (scanTimer < scanInterval) return;
      scanTimer = 0f;

      var font = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
          .FirstOrDefault(f => f.name == "Valheim-Norse");

      // list of all ships that are loaded in the world for this player.
      foreach (var ship in ObjectUtils.EmumerateInstanceOfType<Ship>())
      {
        // Skip if already has a label
        if (ship.GetComponentInChildren<ShipLabelBillboard>() != null) continue;

        Renderer[] renderers = ship.GetComponentsInChildren<Renderer>();
        if (renderers.Length < 0) continue;

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
        Vector3 topLocal = ship.transform.InverseTransformPoint(topWorld);

        // Create label
        var go = new GameObject();
        go.transform.SetParent(ship.transform);
        go.transform.localPosition = topLocal + (Vector3.up * 0.5f) + (Vector3.forward * 0.5f);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        // Create text mesh.
        var textMeshPro = go.AddComponent<TextMeshPro>();
        textMeshPro.font = font;
        textMeshPro.fontSize = 12.5f;
        textMeshPro.color = Color.white;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.enableAutoSizing = false;

        // Attach it to the object.
        Jotunn.Logger.LogDebug("Added ship billboard");
        go.AddComponent<ShipLabelBillboard>();
      }
    }

    protected override void OnUnpatch()
    {
      foreach (var ship in ObjectUtils.EmumerateInstanceOfType<Ship>())
      {
        var billboard = ship.GetComponent<ShipLabelBillboard>();
        if (billboard != null)
        {
          //Object.Destroy(billboard);
        }
      }
      // Set the timer to max to force a rescan immediately on enablement.
      scanTimer = scanInterval;
    }
  }

  class ShipLabelBillboard : MonoBehaviour
  {
    private TextMeshPro textMeshPro;
    private Ship ship;

    void Start()
    {
      textMeshPro = GetComponent<TextMeshPro>();
      ship = GetComponentInParent<Ship>();
    }

    void LateUpdate()
    {
      if (Camera.main == null || ship == null) return;

      // Face the camera
      transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

      // Update text
      string name = ObjectUtils.GetLabelFromObject(ship);
      textMeshPro.text = string.IsNullOrWhiteSpace(name) ? "" : name.Trim();
    }
  }
}