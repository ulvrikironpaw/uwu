using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Extensions;
using Jotunn.Managers;
using UnityEngine;
using UWU.Commands;
using UWU.Common;

namespace UWU.Behaviors
{
  internal abstract class FeatureBehaviour : MonoBehaviour
  {
    protected ConfigEntry<bool> FeatureEnabled { get; set; }
    protected abstract string Name { get; }
    protected abstract string Category { get; }
    protected abstract string Description { get; }
    protected virtual bool EnabledByDefault => true;
    protected virtual bool Synced => true;

    private readonly Harmony harmony;

    protected FeatureBehaviour()
    {
      harmony = new Harmony($"{Manifest.PluginGUID}.{Name.ToLower()}");
    }

    protected virtual void OnConfigure(ConfigFile config) { }

    protected virtual void OnPatch(Harmony harmony) { }

    protected virtual void OnUnpatch() { }

    internal void Configure(ConfigFile config)
    {
      FeatureEnabled = config.BindConfig(
          section: Category,
          key: Name,
          defaultValue: EnabledByDefault,
          description: Description,
          synced: Synced
      );
      // Configure the behavior to start/stop receiving updates.
      enabled = FeatureEnabled.Value;

      CommandManager.Instance.AddConsoleCommand(new BoolCommand(
          name: $"UWU{Name}",
          help: $"Enables or disables the UWU{Name} option",
          adminOnly: Synced,
          isCheat: true,
          () => FeatureEnabled.Value,
          (value) => FeatureEnabled.Value = value
      ));

      FeatureEnabled.SettingChanged += (sender, args) =>
      {
        Jotunn.Logger.LogInfo($"{harmony.Id} settings has changed to " + FeatureEnabled.Value);
        // Either apply patches or unapply them.
        if (FeatureEnabled.Value) Patch();
        else Unpatch();
        // Configure the behavior to start/stop receiving updates.
        enabled = FeatureEnabled.Value;
      };

      OnConfigure(config);

      if (FeatureEnabled.Value)
      {
        Patch();
      }
    }

    private void Patch()
    {
      OnPatch(harmony);
      Jotunn.Logger.LogInfo($"{harmony.Id} is applied");
    }

    private void Unpatch()
    {
      harmony.UnpatchSelf();
      OnUnpatch();
      Jotunn.Logger.LogInfo($"{harmony.Id} is unapplied");
    }
  }
}
