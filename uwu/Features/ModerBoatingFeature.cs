using HarmonyLib;
using UnityEngine;
using UWU.Behaviors;

namespace UWU.Features
{
  internal sealed class ModerBoatingFeature : FeatureBehaviour
  {
    protected override string Name => "ModerBoating";
    protected override string Category => "Sailing";
    protected override string Description => "Permanently applies the Moder buff";
    protected override bool EnabledByDefault => false;

    private const float maxTime = 5f;
    private float updateTimer = maxTime;

    private StatusEffect modifiedModerEffect;

    protected override void OnPatch(Harmony harmony)
    {
      var original = AccessTools.Method(
        typeof(StatusEffect),
        nameof(StatusEffect.Setup));
      var prefix = AccessTools.Method(
        typeof(ModerBoatingFeature),
        nameof(StatusEffect_Setup_Prefix));
      harmony.Patch(original, prefix: new(prefix));
    }

    protected override void OnUnpatch()
    {
      // Prevent null pointer exception if this is called before the player is set.
      var player = Player.m_localPlayer;
      if (player == null) return;

      var seMan = player.GetSEMan();
      if (seMan == null) return;

      // Try to remove Moder effect if possible.
      seMan.RemoveStatusEffect(modifiedModerEffect, false);
    }

    void FixedUpdate()
    {
      // Check every 5 seconds
      updateTimer += Time.deltaTime;
      if (updateTimer < maxTime) return;
      updateTimer = 0f;

      // In the event a local player doesn't exist, short circuit.
      var player = Player.m_localPlayer;
      if (player == null) return;

      // Get Status Effect Manager.
      var seMan = player.GetSEMan();
      if (seMan == null) return;

      // Get the Moder guardian power status effect
      var moderEffect = ObjectDB.instance.GetStatusEffect("GP_Moder".GetHashCode());
      if (moderEffect != null && !seMan.HaveStatusEffect("GP_Moder".GetHashCode()))
      {
        modifiedModerEffect = moderEffect.Clone();
        modifiedModerEffect.m_ttl = 0;
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
