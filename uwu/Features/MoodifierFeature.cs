using BepInEx.Configuration;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using UWU.Behaviors;
using UWU.Commands;
using UWU.Common;
using UWU.Extensions;
using static UWU.Behaviors.Environments;

namespace UWU.Features
{
  internal class MoodifierFeature : FeatureBehaviour
  {
    protected override string Name => "Moodifier";
    protected override string Category => "Weather";
    protected override string Description => "Modifies weather patterns to set the mood";

    private bool weatherConfigured = false;

    protected override void OnConfigure(ConfigFile config)
    {
      CommandManager.Instance.AddConsoleCommand(new StringCommand(
          name: "UWUWeather",
          help: "Attempts to change the local weather to the specified weather name",
          adminOnly: true,
          isCheat: false,
          () => EnvMan.instance?.GetCurrentEnvironment()?.m_name ?? "???",
          (envName) => EnvMan.instance?.SetEnvironment_UWU(envName)));
      CommandManager.Instance.AddConsoleCommand(new VoidCommand(
          name: "UWUListWeather",
          help: "Lists all known weather environments",
          adminOnly: true,
          isCheat: false,
          (args) => EnvMan.instance?.PrintEnvironments_UWU()));
    }

    void FixedUpdate()
    {
      if (weatherConfigured) return;
      if (EnvMan.instance == null) return;
      weatherConfigured = true;

      // Add custom weather patterns.
      AddCustomEnvironments(EnvMan.instance);

      // Reconfigure weather/environment chances per biome.
      ConfigureWeather(EnvMan.instance);
    }

    private static void ConfigureWeather(EnvMan instance)
    {
      foreach (var kvp in GetWeatherOverrides())
      {
        Heightmap.Biome biome = kvp.Key;
        WeatherEntry[] overrides = kvp.Value;
        // Skip configuration if there are no environments.
        if (!overrides.Any())
        {
          Jotunn.Logger.LogWarning($"No overrides for {biome}. Skipping");
          continue;
        }
        // Find the environment setup for the biome.
        var biomeEnvSetup = instance.m_biomes.Find(it => it.m_biome == biome);
        if (biomeEnvSetup == null)
        {
          Jotunn.Logger.LogWarning($"Could not find biome: {biome}");
          continue;
        }
        // Bundle up new entries for the environment
        var envEntries = biomeEnvSetup.m_environments;
        foreach (WeatherEntry entry in overrides)
        {
          var envName = entry.Name;
          var env = instance.m_environments.Find(e => e.m_name.Equals(envName, System.StringComparison.OrdinalIgnoreCase));
          if (env == null)
          {
            Jotunn.Logger.LogWarning($"Environment '{envName}' not found!");
            continue;
          }
          // Each environment gets equal chance
          var existingEntry = envEntries.Find(it => it.m_env == env);
          if (existingEntry == null)
          {
            envEntries.Add(new EnvEntry { m_env = env, m_weight = entry.Weight });
          } 
          else
          {
            existingEntry.m_weight = entry.Weight;
          }
          Jotunn.Logger.LogInfo($"Added '{envName}' to {biome} biome weather pool.");
        }
      }
    }

    private static IDictionary<Heightmap.Biome, WeatherEntry[]> GetWeatherOverrides()
    {
      return new Dictionary<Heightmap.Biome, WeatherEntry[]>
      {
        {
          Heightmap.Biome.Meadows,
          new WeatherEntry[]
          {
            new() { Name = EnvId.Clear, Weight = 3.5f },
            new() { Name = EnvId.PlainsClear, Weight = 0.8f },
            new() { Name = EnvId.LightRain, Weight = 0.8f },
            new() { Name = EnvId.MeadowsBoss, Weight = 0.2f },
            new() { Name = EnvId.Misty, Weight = 0.2f },
            new() { Name = EnvId.Overcast, Weight = 0.5f },
            new() { Name = EnvId.Rain, Weight = 0.4f },
            new() { Name = EnvId.Thunderstorm, Weight = 0.2f },
          }
        },
        {
          Heightmap.Biome.BlackForest,
          new WeatherEntry[]
          {
            new() { Name = EnvId.BlackForestMist, Weight = 2.5f },
            new() { Name = EnvId.EtherealMist, Weight = 0.4f },
            new() { Name = EnvId.LightRain, Weight = 0.2f },
            new() { Name = EnvId.MistlandsRain, Weight = 0.1f },
            new() { Name = EnvId.Misty, Weight = 0.2f },
            new() { Name = EnvId.Overcast, Weight = 0.4f },
            new() { Name = EnvId.Rain, Weight = 0.2f },
            new() { Name = EnvId.Thunderstorm, Weight = 0.3f },
            new() { Name = EnvId.ThunderStormNoFog, Weight = 0.3f },
          }
        },
        {
          Heightmap.Biome.Swamp,
          new WeatherEntry[]
          {
            new() { Name = EnvId.BlackForestMist, Weight = 1f },
            new() { Name = EnvId.LightRain, Weight = 0.6f },
            new() { Name = EnvId.MistlandsRain, Weight = 0.3f },
            new() { Name = EnvId.Misty, Weight = 1.2f },
            new() { Name = EnvId.Overcast, Weight = 1f },
            new() { Name = EnvId.Rain, Weight = 0.4f },
            new() { Name = EnvId.SwampBoss, Weight = 0.8f },
            new() { Name = EnvId.SwampRain, Weight = 0.4f },
            new() { Name = EnvId.Thunderstorm, Weight = 0.4f },
            new() { Name = EnvId.ThunderStormNoFog, Weight = 0.4f },
          }
        },
        {
          Heightmap.Biome.Mountain,
          new WeatherEntry[]
          {
            new() { Name = EnvId.Clear, Weight = 1f },
            new() { Name = EnvId.FrostFog, Weight = 1f },
            new() { Name = EnvId.OvercastSnow, Weight = 1.5f },
            new() { Name = EnvId.Snow, Weight = 3.0f },
            new() { Name = EnvId.SnowStorm, Weight = 1.2f },
            new() { Name = EnvId.TwilightSnowStorm, Weight = 0.6f },
          }
        },
        {
          Heightmap.Biome.Plains,
          new WeatherEntry[]
          {
            new() { Name = EnvId.DustStorm, Weight = 0.2f },
            new() { Name = EnvId.GoldenDusk, Weight = 0.6f },
            new() { Name = EnvId.PlainsClear, Weight = 3.5f },
            new() { Name = EnvId.LightRain, Weight = 0.8f },
            new() { Name = EnvId.MeadowsBoss, Weight = 0.2f },
            new() { Name = EnvId.Misty, Weight = 0.4f },
            new() { Name = EnvId.Overcast, Weight = 0.5f },
            new() { Name = EnvId.Thunderstorm, Weight = 0.2f },
          }
        },
        {
          Heightmap.Biome.Ocean,
          new WeatherEntry[]
          {
            new() { Name = EnvId.Clear, Weight = 2.6f },
            new() { Name = EnvId.EtherealMist, Weight = 0.3f },
            new() { Name = EnvId.LightRain, Weight = 0.6f },
            new() { Name = EnvId.Misty, Weight = 0.1f },
            new() { Name = EnvId.MistlandsThunder, Weight = 0.1f },
            new() { Name = EnvId.Overcast, Weight = 1.2f },
            new() { Name = EnvId.Rain, Weight = 0.4f },
            new() { Name = EnvId.SeaSquall, Weight = 0.5f },
            new() { Name = EnvId.Thunderstorm, Weight = 0.3f },
            new() { Name = EnvId.ThunderStormNoFog, Weight = 0.3f },
          }
        },
        {
          Heightmap.Biome.Mistlands,
          new WeatherEntry[]
          {
            new() { Name = EnvId.Clear, Weight = 1.0f },
            new() { Name = EnvId.EtherealMist, Weight = 0.4f },
            new() { Name = EnvId.MeadowsBoss, Weight = 0.2f },
            new() { Name = EnvId.MistlandsClear, Weight = 1.0f },
            new() { Name = EnvId.MistlandsRain, Weight = 0.15f },
            new() { Name = EnvId.MistlandsThunder, Weight = 0.2f },
            new() { Name = EnvId.Overcast, Weight = 0.3f },
            new() { Name = EnvId.Rain, Weight = 0.15f },
          }
        }
      };
    }

  }

  /// <summary>
  /// Structured data for configuring biomes.
  /// </summary>
  class WeatherEntry
  {
    /// <summary>
    /// The environment to use (Clear, Thunderstorm, etc.)
    /// </summary>
    internal string Name { get; set; }

    /// <summary>
    /// The weight to apply to this particular environment in this biome.
    /// </summary>
    internal float Weight { get; set; }
  }
}
