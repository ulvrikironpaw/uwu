using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UWU.Extensions
{
  internal static class EnvManExtensions
  {
    private static readonly Action<EnvMan, string> QueueEnvironmentDelegate;

    static EnvManExtensions()
    {
      var originalQueueEnvironment = typeof(EnvMan).GetMethod(
        "QueueEnvironment",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null,
        new[] { typeof(string) },
        null);
      QueueEnvironmentDelegate = AccessTools
        .MethodDelegate<Action<EnvMan, string>>(originalQueueEnvironment);
    }

    /// <summary>
    /// Queues up the next environment.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="envName"></param>
    internal static void QueueEnvironment_UWU(this EnvMan self, string envName)
    {
      QueueEnvironmentDelegate(self, envName);
    }

    /// <summary>
    /// Prints all environments to the console.
    /// </summary>
    /// <param name="self"></param>
    internal static void PrintEnvironments_UWU(this EnvMan self)
    {
      var environments = self.m_environments;
      if (environments == null || environments.Count == 0)
      {
        Console.instance.Print("[UWU] No environments found!");
        return;
      }

      Console.instance.Print("[UWU] --- Known Weather Environments ---");
      foreach (var env in environments.OrderBy(it => it.m_name))
      {
        // Show name + quick summary flags
        var flags = "";
        if (env.m_isFreezing) flags += " ❄️";
        if (env.m_isCold && !env.m_isFreezing) flags += " 🧊";
        if (env.m_isWet) flags += " 💧";

        Console.instance.Print($"[UWU]  • {env.m_name}{flags}");
      }
    }

    internal static void PrintBiomeEnvironmentConfig(this EnvMan self)
    {
      if (self == null)
      {
        Jotunn.Logger.LogWarning("[UWU] EnvMan not initialized yet!");
        return;
      }

      var sb = new StringBuilder();
      sb.AppendLine("=== UWU Biome Weather Overview ===");

      foreach (var biomeSetup in self.m_biomes)
      {
        if (biomeSetup == null || biomeSetup.m_environments == null || biomeSetup.m_environments.Count == 0)
          continue;

        float totalWeight = biomeSetup.m_environments.Sum(e => e.m_weight);
        sb.AppendLine($"\nBiome: {biomeSetup.m_biome}");

        foreach (var envEntry in biomeSetup.m_environments)
        {
          string envName = envEntry.m_env?.m_name ?? "(null)";
          float percent = (envEntry.m_weight / totalWeight) * 100f;
          sb.AppendLine($"  - {envName}: {envEntry.m_weight:F2} weight → {percent:F1}%");
        }
      }

      Jotunn.Logger.LogInfo(sb.ToString());
    }

    /// <summary>
    /// Queues a weather change. Intended largely for debugging.
    /// </summary>
    /// <param name="envMan"></param>
    /// <param name="envName"></param>
    internal static void SetEnvironment_UWU(this EnvMan envMan, string envName)
    {
      // Validate the environment exists
      var nameToLower = envName.ToLowerInvariant();
      var matchingEnvs = envMan.m_environments.FindAll(
        e => e.m_name.ToLowerInvariant().Contains(nameToLower));
      if (matchingEnvs.Count < 1)
      {
        Console.instance.Print($"No environment named '{envName}' found!");
        return;
      }

      if (matchingEnvs.Count == 1)
      {
        var foundWeatherName = matchingEnvs[0].m_name;
        envMan.QueueEnvironment_UWU(foundWeatherName);
        Console.instance.Print($"UWU: Weather set to '{foundWeatherName}' 💨");
        return;
      }

      var exactMatch = matchingEnvs.FirstOrDefault(it =>
        it.m_name.ToLowerInvariant() == nameToLower);
      if (exactMatch != null)
      {
        envMan.QueueEnvironment_UWU(exactMatch.m_name);
        Console.instance.Print($"UWU: Weather set to '{exactMatch.m_name}' 💨");
        return;
      }

      foreach (var env in matchingEnvs.OrderBy(it => it.m_name))
      {
        Console.instance.Print($"[UWU]  • {env.m_name}");
      }
      Console.instance.Print($"Too many matching environments. Be specific");
    }
  }
}
