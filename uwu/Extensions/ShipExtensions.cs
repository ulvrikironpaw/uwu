using HarmonyLib;
using System;
using System.Reflection;

namespace UWU.Extensions
{
  internal static class ShipExtensions
  {
    private static readonly Func<Ship, bool> HaveControllingPlayerDelegate;

    private static readonly AccessTools.FieldRef<Ship, Ship.Speed> m_speedRef = AccessTools.FieldRefAccess<Ship, Ship.Speed>("m_speed");

    static ShipExtensions()
    {
      var originalHaveControllingPlayer = typeof(Ship).GetMethod("HaveControllingPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
      HaveControllingPlayerDelegate = AccessTools.MethodDelegate<Func<Ship, bool>>(originalHaveControllingPlayer);
    }

    internal static bool GetControllingPlayer_UWU(this Ship ship)
    {
      return HaveControllingPlayerDelegate(ship);
    }

    internal static Ship.Speed GetSpeed_UWU(this Ship ship)
    {
      return m_speedRef(ship);
    }
  }
}
