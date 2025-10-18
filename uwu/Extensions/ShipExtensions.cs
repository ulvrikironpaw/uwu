using HarmonyLib;
using System;
using System.Reflection;

namespace UWU.Extensions
{
  internal static class ShipExtensions
  {
    private static readonly Func<Ship, bool> HaveControllingPlayerDelegate;

    static ShipExtensions()
    {
      var originalHaveControllingPlayer = typeof(Ship).GetMethod("HaveControllingPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
      HaveControllingPlayerDelegate = AccessTools.MethodDelegate<Func<Ship, bool>>(originalHaveControllingPlayer);
    }

    public static bool GetControllingPlayer_UWU(this Ship ship)
    {
      return HaveControllingPlayerDelegate(ship);
    }
  }
}
