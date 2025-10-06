using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Extensions;
using Jotunn.Managers;

namespace UWU.Common
{
    internal abstract class UWUFeature
    {
        internal ConfigEntry<bool> Enabled { get; set; }
        internal abstract string Name { get; }
        protected abstract string Category { get; }
        protected abstract string Description { get; }
        protected virtual bool EnabledByDefault => true;
        protected virtual bool Synced => true;

        private readonly Harmony harmony;

        private bool wasEnabled = false;

        protected UWUFeature()
        {
            harmony = new Harmony($"{Manifest.PluginGUID}.{Name.ToLower()}");
            Jotunn.Logger.LogInfo($"{harmony.Id} is instantiated");
        }

        protected virtual void OnConfigure(ConfigFile config) { }

        protected virtual void OnPatch(Harmony harmony) { }

        protected virtual void OnUnpatch() { }

        protected virtual void OnGUI() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnDestroy() { }

        internal void Configure(ConfigFile config)
        {
            Enabled = config.BindConfig(
                section: Category,
                key: Name,
                defaultValue: EnabledByDefault,
                description: Description,
                synced: Synced
            );
            wasEnabled = true;

            CommandManager.Instance.AddConsoleCommand(new BoolCommand(
                name: $"UWU{Name}",
                help: $"Enables or disables the UWU{Name} option",
                adminOnly: Synced,
                isCheat: false,
                () => Enabled.Value,
                (value) => Enabled.Value = value
            ));

            OnConfigure(config);
            if (Enabled.Value)
            {
                Patch();
            }
            Jotunn.Logger.LogInfo($"{harmony.Id} is configured");
        }

        internal void Update()
        {
            if (!Enabled.Value) return;
            OnUpdate();
        }

        internal void LateUpdate()
        {
            // Don't fix patches if the enablement hasn't changed.
            if (Enabled.Value == wasEnabled) return;

            // Update the was enabled value so it can be used for change detection.
            Jotunn.Logger.LogInfo($"{harmony.Id} enablement changed {wasEnabled} -> {Enabled.Value}");
            wasEnabled = Enabled.Value;
            if (Enabled.Value) Patch();
            else Unpatch();
        }

        internal void UpdateGUI()
        {
            if (!Enabled.Value) return;

            OnGUI();
        }

        internal void Destroy()
        {
            Unpatch();
            OnDestroy();
        }

        private void Patch()
        {
            // Defensive unpatch if necessary.
            OnPatch(harmony);
            Jotunn.Logger.LogInfo($"{harmony.Id} is patched");
        }

        private void Unpatch()
        {
            harmony.UnpatchSelf();
            OnUnpatch();
            Jotunn.Logger.LogInfo($"{harmony.Id} is unpatched");
        }

    }
}
