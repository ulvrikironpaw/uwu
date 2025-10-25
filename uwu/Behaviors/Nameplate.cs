using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UWU.Common;
using UWU.Extensions;

namespace UWU.Behaviors
{
  /// <summary>
  /// Adds a billboard to the parent object.
  /// </summary>
  internal class Nameplate : MonoBehaviour
  {

    // Must be set after AddComponent in the parent.
    internal MonoBehaviour target;
    internal GameObject parentObject;
    private TextMeshPro textMeshPro;

    IEnumerator Start()
    {
      // Create text mesh.
      textMeshPro = gameObject.AddComponent<TextMeshPro>();
      textMeshPro.font = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
          .FirstOrDefault(f => f.name == "Valheim-Norse");
      textMeshPro.fontStyle = FontStyles.Bold;
      textMeshPro.fontSize = 9f;
      // White.
      textMeshPro.color = new Color(1f, 1f, 1f);
      textMeshPro.alignment = TextAlignmentOptions.Center;
      textMeshPro.enableAutoSizing = false;

      var mat = new Material(textMeshPro.font.material);
      mat.EnableKeyword("OUTLINE_ON");
      mat.SetColor("_OutlineColor", new Color(0f, 0f, 0.3f, 1f)); // Dark blue
      mat.SetFloat("_OutlineWidth", 0.085f);
      textMeshPro.fontMaterial = mat;

      // The object has to be around long enough for physics to be applied.
      // Just wait 2.5 seconds since that is probably long enough for a save
      // operation which would disrupt.
      yield return new WaitForSeconds(2f);

      var attachTransform = target switch
      {
        Ship =>
          target.transform?.Find("ship/visual/mast/pillar") // Rafts
          ?? target.transform?.Find("ship/visual/Mast/mast") // Longships/VikingShips
          ?? target.transform?.Find("ship/mast"), // Else.
        _ => null,
      };
      if (attachTransform == null)
      {
        Jotunn.Logger.LogInfo($"Unable to find nameplate anchor for {NameCache.GetLabelFromObject(target)}, falling back to transform");
        attachTransform = target.transform;
      }

      if (attachTransform == null)
      {
        // This can happen if something doesn't live long enough (goes out of scope in a fast moving ship)
        yield break;
      }

      var topWorld = PhysicsUtils.GetTopCenterPoint(attachTransform);

      parentObject.transform.position = topWorld + (Vector3.up * 0.5f);
      parentObject.transform.SetParent(target.transform, worldPositionStays: true);
    }

    void LateUpdate()
    {
      if (Camera.main == null || target == null || textMeshPro == null) return;

      // If we don't actually have a local player, there isn't a reason to even
      // do the rest of the logic.
      var localPlayer = Player.m_localPlayer;
      if (localPlayer == null) return;

      // Special handling if it is a ship. The nameplates are kind of annoying while you are actually sailing.
      if (target is Ship ship)
      {
        // Skip all other updates since the mesh isn't visible.
        var isOnShip = localPlayer.GetControlledShip() == ship || localPlayer.GetStandingOnShip() == ship;
        var hasControllingPlayer = ship.GetControllingPlayer_UWU();
        var isNameplateVisible = !isOnShip || !hasControllingPlayer;
        textMeshPro.enabled = isNameplateVisible;
        if (!isNameplateVisible) return;
      }

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
      string name = NameCache.GetLabelFromObject(target);
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
        Decorate(target);
      }
    }

    internal static void Decorate(MonoBehaviour target)
    {
      var go = new GameObject();
      go.transform.localScale = Vector3.one;

      var nameplate = go.AddComponent<Nameplate>();
      nameplate.target = target;
      nameplate.parentObject = go;
    }
  }
}
