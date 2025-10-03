using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Extensions;
using Jotunn.Managers;
using UnityEngine;

namespace UWU
{
    internal class PermanentBuffFeature
    {
        private static ConfigEntry<bool> EnablePermanentModer;
        private const float maxTime = 5f;
        private static float updateTimer = maxTime;

        internal static void Configure(ConfigFile config)
        {
            EnablePermanentModer = config.BindConfig(
                section: "Sailing",
                key: "PermanentModer",
                defaultValue: true,
                description: "Permanently applies the Moder buff",
                synced: true
            );

            CommandManager.Instance.AddConsoleCommand(new BoolConsoleCommand(
                name: "UWUPermanentModer",
                help: "Enables or disables the UWU.PermanentModer option",
                adminOnly: true,
                isCheat: false,
                () => EnablePermanentModer.Value,
                (value) => EnablePermanentModer.Value = value
            ));
        }

        internal static void Patch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(StatusEffect), nameof(StatusEffect.Setup));
            var prefix = AccessTools.Method(typeof(PermanentBuffFeature), nameof(StatusEffect_Setup_Prefix));
            harmony.Patch(original, prefix: new(prefix));
        }

        internal static void Update()
        {
            if (!EnablePermanentModer.Value)
            {
                // Cancel when disabled.
                return;
            }

            // Check every 60 seconds
            updateTimer += Time.deltaTime;
            if (updateTimer < maxTime) return;
            updateTimer = 0f;

            // In the event a local player doesn't exist, short circuit.
            var player = Player.m_localPlayer;
            if (player == null)
            {
                return;
            }

            // Get Status Effect Manager.
            var seMan = player.GetSEMan();
            if (seMan == null)
            {
                return;
            }
            // Get the Moder guardian power status effect
            var moderEffect = ObjectDB.instance.GetStatusEffect("GP_Moder".GetHashCode());
            if (moderEffect != null && !seMan.HaveStatusEffect("GP_Moder".GetHashCode()))
            {
                StatusEffect modifiedModerEffect = moderEffect.Clone();
                modifiedModerEffect.m_ttl = 0; // never expires
                modifiedModerEffect.m_isNew = false;
                seMan.AddStatusEffect(modifiedModerEffect, false);
            }
        }

        private static bool StatusEffect_Setup_Prefix(StatusEffect __instance, Character character)
        {
            if (__instance.m_name == "$se_moder_name")
            {
                // Setup the character but ignore the effects for Modor.
                __instance.m_character = character;
                return false;
            }
            return true;
        }
    }
}
