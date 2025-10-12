using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UWU.Common;

namespace UWU.Features
{
    internal class BoatyMcBoatfaceFeature : UWUFeature
    {
        private static BoatyMcBoatfaceFeature instance;

        internal BoatyMcBoatfaceFeature()
        {
            instance = this;
        }

        internal override string Name => "BoatyMcBoatface";
        protected override string Category => "Sailing";
        protected override string Description => "Adds a shortcut key to rename boats and optionally auto-names them";
        protected override bool EnabledByDefault => true;

        private ConfigEntry<KeyboardShortcut> RenameShortcut;


        protected override void OnConfigure(ConfigFile config)
        {
            RenameShortcut = config.Bind(
                "Sailing",
                "RenameKey",
                new KeyboardShortcut(KeyCode.N),
                "Key used for renaming ships");
        }

        protected override void OnPatch(Harmony harmony)
        {
            var getHoverTextOriginal = AccessTools.Method(typeof(ShipControlls), nameof(ShipControlls.GetHoverText));
            var getHoverTextPostfix = AccessTools.Method(typeof(BoatyMcBoatfaceFeature), nameof(ShipControlls_GetHoverText_Postfix));
            harmony.Patch(getHoverTextOriginal, postfix: new(getHoverTextPostfix));
        }

        private static void ShipControlls_GetHoverText_Postfix(ShipControlls __instance, ref string __result)
        {
            string keyName = instance.RenameShortcut.Value.MainKey.ToString();
            __result += $"\n[<color=yellow><b>{keyName}</b></color>] Rename ship";

            if (!instance.RenameShortcut.Value.IsDown() || PromptUtils.IsDialogOpen()) return;

            var ship = __instance.m_ship;
            if (ship == null) return;

            var zNetView = ObjectUtils.GetZNetView(ship);
            if (zNetView == null || !zNetView.IsValid()) return;

            var zdo = zNetView.GetZDO();
            if (zdo == null) return;

            string currentName = ObjectUtils.GetLabelFromZNetView(zNetView) ?? "";

            PromptUtils.LaunchDialog($"Rename ship {zdo.m_uid.ID}", currentName, (newTitle) =>
            {
                RPCManager.RenameObject(zdo, newTitle);
                Player.m_localPlayer?.Message(MessageHud.MessageType.Center,
                    $"Ship renamed to {newTitle}");
            });
        }
    }
}