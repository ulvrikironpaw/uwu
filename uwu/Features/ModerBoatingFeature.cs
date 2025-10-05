using HarmonyLib;
using UnityEngine;
using UWU.Common;

namespace UWU.Features
{
    internal sealed class ModerBoatingFeature : UWUFeature
    {
        private static ModerBoatingFeature instance;

        internal override string Name => "ModerBoating";
        protected override string Category => "Sailing";
        protected override string Description => "Permanently applies the Moder buff";

        private const float maxTime = 5f;
        private float updateTimer = maxTime;

        internal ModerBoatingFeature()
        {
            instance = this;
        }

        protected override void OnPatch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(StatusEffect), nameof(StatusEffect.Setup));
            var prefix = AccessTools.Method(typeof(ModerBoatingFeature), nameof(StatusEffect_Setup_Prefix));
            harmony.Patch(original, prefix: new(prefix));
        }

        protected override void OnUpdate()
        {
            if (!Enabled.Value) return;

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
