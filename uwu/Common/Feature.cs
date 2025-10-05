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


        internal void Configure(ConfigFile config)
        {
            Enabled = config.BindConfig(
                section: Category,
                key: Name,
                defaultValue: EnabledByDefault,
                description: Description,
                synced: Synced
            );

            CommandManager.Instance.AddConsoleCommand(new BoolCommand(
                name: $"UWU{Name}",
                help: $"Enables or disables the UWU{Name} option",
                adminOnly: Synced,
                isCheat: false,
                () => Enabled.Value,
                (value) => Enabled.Value = value
            ));

            OnConfigure(config);
            Jotunn.Logger.LogInfo($"{harmony.Id} is configured");
        }

        internal void Update()
        {
            OnUpdate();
        }

        internal void UpdateGUI()
        {
            OnGUI();
        }

        internal void Patch()
        {
            OnPatch(harmony);
            Jotunn.Logger.LogInfo($"{harmony.Id} is patched");
        }

        internal void Unpatch()
        {
            harmony.UnpatchSelf();
            OnUnpatch();
            Jotunn.Logger.LogInfo($"{harmony.Id} is unpatched");
        }
    }
}
