using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWU.Common;

namespace UWU.Behaviors
{
  /// <summary>
  /// Configure Custom Environments.
  /// </summary>
  internal class Environments
  {
    internal static void AddCustomEnvironments(EnvMan instance)
    {
      foreach (var env in GetUnregistedEnvironments(instance))
      {
        if (env == null) continue;
        instance.m_environments.Add(env);
        Jotunn.Logger.LogInfo($"Added new environment: {env.m_name}");
      }
    }

    private static IEnumerable<EnvSetup> GetUnregistedEnvironments(EnvMan instance)
    {
      yield return CreateOvercastEnvironment(instance);
      yield return CreateOvercastSnowEnvironment(instance);
      yield return CreateFrostFogEnvironment(instance);
      yield return CreateGoldenDuskEnvironment(instance);
      yield return CreateDustStormEnvironment(instance);
      yield return CreateSeaSquallEnvironment(instance);
      yield return CreateEtherealMistEnvironment(instance);
    }

    private static EnvSetup CreateOvercastEnvironment(EnvMan instance)
    {
      if (instance.m_environments.Any(it => it.m_name == EnvId.Overcast)) return null;

      var rainEnv = instance.m_environments.Find(e => e.m_name == "Rain");
      if (rainEnv == null)
      {
        Jotunn.Logger.LogWarning("Could not find base environment 'Rain'!");
        return null;
      }
      var clearEnv = instance.m_environments.Find(e => e.m_name == "Clear");
      if (clearEnv == null)
      {
        Jotunn.Logger.LogWarning("Could not find base environment 'Clear'!");
        return null;
      }

      // Clone it
      var env = rainEnv.Clone();
      env.m_name = EnvId.Overcast;
      env.m_rainCloudAlpha = 0.5f;
      env.m_fogDensityDay = 0.002f;
      env.m_fogDensityNight = 0.005f;
      env.m_windMin = 0.1f;
      env.m_windMax = 0.25f;
      env.m_rainCloudAlpha = 0.35f;
      env.m_isWet = false;
      env.m_alwaysDark = false;
      env.m_fogColorDay = new Color(0.65f, 0.65f, 0.68f, 1f);
      env.m_fogColorEvening = new Color(0.55f, 0.55f, 0.6f, 1f);
      env.m_fogColorNight = new Color(0.4f, 0.4f, 0.45f, 1f);
      env.m_fogColorSunDay = new Color(0.8f, 0.8f, 0.85f, 1f);

      // Copy partical systems from the Plains.
      env.m_ambientLoop = clearEnv.m_ambientLoop;
      env.m_psystems = clearEnv.m_psystems?.ToArray() ?? new GameObject[0];
      return env;
    }

    private static EnvSetup CreateOvercastSnowEnvironment(EnvMan instance)
    {
      if (instance.m_environments.Any(it => it.m_name == EnvId.OvercastSnow)) return null;

      var baseEnv = instance.m_environments.Find(e => e.m_name == "Snow") ??
                    instance.m_environments.Find(e => e.m_name == "SnowStorm");

      if (baseEnv == null)
      {
        Jotunn.Logger.LogWarning("Could not find base environment 'Snow' or 'SnowStorm'!");
        return null;
      }

      var env = baseEnv.Clone();
      env.m_name = EnvId.OvercastSnow;
      env.m_rainCloudAlpha = 0.4f;
      env.m_fogDensityDay = 0.003f;
      env.m_fogDensityNight = 0.006f;
      env.m_windMin = 0.05f;
      env.m_windMax = 0.2f;
      env.m_isWet = false;
      env.m_isFreezing = true;
      env.m_fogColorDay = new Color(0.75f, 0.75f, 0.8f, 1f);
      env.m_fogColorEvening = new Color(0.65f, 0.65f, 0.7f, 1f);
      env.m_fogColorNight = new Color(0.5f, 0.5f, 0.55f, 1f);
      env.m_fogColorSunDay = new Color(0.8f, 0.8f, 0.85f, 1f);
      env.m_sunColorDay = new Color(0.9f, 0.9f, 1f, 1f);
      return env;
    }

    private static EnvSetup CreateFrostFogEnvironment(EnvMan instance)
    {
      if (instance.m_environments.Any(it => it.m_name == EnvId.FrostFog)) return null;

      var baseEnv = instance.m_environments.Find(e => e.m_name == "Snow");
      if (baseEnv == null)
      {
        Jotunn.Logger.LogWarning("Could not find base environment 'Snow'!");
        return null;
      }

      var env = baseEnv.Clone();
      env.m_name = EnvId.FrostFog;
      env.m_rainCloudAlpha = 0.2f;
      env.m_fogDensityDay = 0.006f;
      env.m_fogDensityNight = 0.012f;
      env.m_windMin = 0.0f;
      env.m_windMax = 0.1f;
      env.m_isWet = false;
      env.m_isFreezing = true;
      env.m_psystems = Array.Empty<GameObject>(); // remove snow
      env.m_fogColorDay = new Color(0.7f, 0.8f, 0.9f, 1f);
      env.m_fogColorEvening = new Color(0.6f, 0.7f, 0.8f, 1f);
      env.m_fogColorNight = new Color(0.45f, 0.5f, 0.55f, 1f);
      env.m_fogColorSunDay = new Color(0.9f, 0.95f, 1f, 1f);
      env.m_sunColorDay = new Color(0.8f, 0.85f, 1f, 1f);
      return env;
    }

    private static EnvSetup CreateGoldenDuskEnvironment(EnvMan instance)
    {
      if (instance.m_environments.Any(it => it.m_name == EnvId.GoldenDusk)) return null;

      var baseEnv = instance.m_environments.Find(e => e.m_name == "Heath clear");
      if (baseEnv == null)
      {
        Jotunn.Logger.LogWarning("Could not find base environment 'Heath clear'!");
        return null;
      }

      var env = baseEnv.Clone();
      env.m_name = EnvId.GoldenDusk;
      env.m_fogColorDay = new Color(1.0f, 0.85f, 0.6f, 1f); // golden
      env.m_fogColorEvening = new Color(1.0f, 0.7f, 0.4f, 1f); // sunset orange
      env.m_fogColorNight = new Color(0.4f, 0.3f, 0.25f, 1f); // warm brownish night
      env.m_fogColorSunDay = new Color(1.1f, 0.9f, 0.7f, 1f);
      env.m_fogDensityDay = 0.0015f;
      env.m_fogDensityNight = 0.0025f;
      env.m_lightIntensityDay = 1.1f;
      env.m_lightIntensityNight = 0.8f;
      env.m_windMin = 0.05f;
      env.m_windMax = 0.2f;
      env.m_rainCloudAlpha = 0.15f;
      env.m_isWet = false;
      env.m_isColdAtNight = true;
      return env;
    }

    private static EnvSetup CreateSeaSquallEnvironment(EnvMan instance)
    {
      if (instance.m_environments.Any(it => it.m_name == EnvId.SeaSquall)) return null;

      var stormEnv = instance.m_environments.Find(e => e.m_name == EnvId.Thunderstorm);
      if (stormEnv == null)
      {
        Jotunn.Logger.LogWarning("Could not find base environment 'ThunderStorm'!");
        return null;
      }

      // Fierce, chaotic, meow.
      var env = stormEnv.Clone();
      env.m_name = EnvId.SeaSquall;
      env.m_rainCloudAlpha = 0.85f;
      env.m_fogDensityDay = 0.03f;
      env.m_fogDensityNight = 0.045f;
      env.m_fogColorDay = new Color(0.25f, 0.3f, 0.35f, 1f);
      env.m_fogColorNight = new Color(0.1f, 0.12f, 0.15f, 1f);
      env.m_ambColorDay = new Color(0.35f, 0.35f, 0.4f, 1f);
      env.m_ambColorNight = new Color(0.15f, 0.18f, 0.22f, 1f);
      env.m_windMin = 0.8f;
      env.m_windMax = 1.4f;
      env.m_rainCloudAlpha = 0.8f;
      env.m_isWet = true;
      env.m_isCold = true;
      env.m_alwaysDark = true;
      return env;
    }

    private static EnvSetup CreateDustStormEnvironment(EnvMan instance)
    {
      if (instance.m_environments.Any(it => it.m_name == EnvId.DustStorm)) return null;

      var baseEnv = instance.m_environments.Find(e => e.m_name == "ThunderStorm");
      if (baseEnv == null)
      {
        Jotunn.Logger.LogWarning("Could not find base environment 'ThunderStorm'!");
        return null;
      }

      var env = baseEnv.Clone();
      env.m_name = EnvId.DustStorm;
      env.m_fogColorDay = new Color(0.8f, 0.7f, 0.5f, 1f);
      env.m_fogColorEvening = new Color(0.7f, 0.55f, 0.35f, 1f);
      env.m_fogColorNight = new Color(0.4f, 0.3f, 0.2f, 1f);
      env.m_fogColorSunDay = new Color(1.0f, 0.85f, 0.55f, 1f);
      env.m_fogDensityDay = 0.01f;
      env.m_fogDensityNight = 0.012f;
      env.m_windMin = 0.4f;
      env.m_windMax = 0.7f;
      env.m_isWet = false;
      env.m_rainCloudAlpha = 0.4f;
      env.m_psystems = new GameObject[0];
      env.m_lightIntensityDay = 0.7f;
      env.m_lightIntensityNight = 0.6f;
      return env;
    }

    private static EnvSetup CreateEtherealMistEnvironment(EnvMan instance)
    {
      if (instance.m_environments.Any(it => it.m_name == EnvId.EtherealMist)) return null;

      var baseEnv = instance.m_environments.Find(e => e.m_name == "Mistlands_clear");
      if (baseEnv == null)
      {
        Jotunn.Logger.LogWarning("Base environment 'Mistlands_clear' not found!");
        return null;
      }

      var env = baseEnv.Clone();
      env.m_name = EnvId.EtherealMist;
      env.m_fogDensityDay = 0.015f;
      env.m_fogDensityNight = 0.02f;
      env.m_fogColorDay = new Color(0.65f, 0.7f, 0.8f, 1f);
      env.m_fogColorNight = new Color(0.45f, 0.5f, 0.6f, 1f);
      env.m_fogColorSunDay = new Color(0.8f, 0.85f, 0.9f, 1f);
      env.m_windMin = 0.05f;
      env.m_windMax = 0.15f;
      return env;
    }
  }
}
