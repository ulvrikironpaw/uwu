using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UWU.Behaviors;
using UWU.Common;

namespace UWU.Features
{
  internal class ShipRenameFeature : FeatureBehaviour
  {
    protected static ShipRenameFeature instance;

    protected override string Name => "ShipRename";
    protected override string Category => "Sailing";
    protected override string Description => "Allows renaming ships";
    protected override bool EnabledByDefault => true;

    private ConfigEntry<KeyboardShortcut> RenameShortcut;

    internal ShipRenameFeature()
    {
      instance = this;
    }

    protected override void OnConfigure(ConfigFile config)
    {
      RenameShortcut = config.Bind(
          "Sailing",
          "ShipRenameKey",
          new KeyboardShortcut(KeyCode.N),
          "Key used for renaming ships");
    }

    protected override void OnPatch(Harmony harmony)
    {
      var getHoverTextOriginal = AccessTools.Method(
        typeof(ShipControlls),
        nameof(ShipControlls.GetHoverText));
      var getHoverTextPostfix = AccessTools.Method(
        typeof(ShipRenameFeature),
        nameof(ShipControlls_GetHoverText_Postfix));
      harmony.Patch(getHoverTextOriginal, postfix: new(getHoverTextPostfix));
    }

    private static void ShipControlls_GetHoverText_Postfix(ShipControlls __instance, ref string __result)
    {
      string keyName = instance.RenameShortcut.Value.MainKey.ToString();
      __result = $"{NameCache.GetLabelFromObject(__instance.m_ship)}\n\n"
        + $"{__result}\n"
        + $"[<color=yellow><b>{keyName}</b></color>] Rename";

      if (!instance.RenameShortcut.Value.IsDown() || UserHud.IsDialogOpen()) return;

      var ship = __instance.m_ship;
      if (ship == null) return;

      var zNetView = ObjectUtils.GetZNetView(ship);
      if (zNetView == null || !zNetView.IsValid()) return;

      var zdo = zNetView.GetZDO();
      if (zdo == null) return;

      string currentName = NameCache.GetLabelFromZNetView(zNetView) ?? "";

      UserHud.Confirm($"Rename ship", currentName, (newTitle) =>
      {
        RPCManager.RenameObject(zdo, newTitle);
        UserHud.Alert($"Ship renamed to {newTitle}");
      });
    }
  }
}