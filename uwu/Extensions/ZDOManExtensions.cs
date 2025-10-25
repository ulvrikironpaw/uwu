using HarmonyLib;
using System.Collections.Generic;

namespace UWU.Extensions
{
  internal static class ZDOManExtensions
  {
    private static readonly AccessTools.FieldRef<ZDOMan, Dictionary<ZDOID, ZDO>> ObjectsByIdRef =
      AccessTools.FieldRefAccess<ZDOMan, Dictionary<ZDOID, ZDO>>("m_objectsByID");

    private static readonly AccessTools.FieldRef<ZDOMan, List<ZDOID>> DestroySendListRef =
      AccessTools.FieldRefAccess<ZDOMan, List<ZDOID>>("m_destroySendList");

    public static Dictionary<ZDOID, ZDO> GetObjectsById_UWU(this ZDOMan zdoMan)
    {
      return ObjectsByIdRef(zdoMan);
    }

    public static List<ZDOID> GetDestroyedSendList_UWU(this ZDOMan zdoMan)
    {
      return DestroySendListRef(zdoMan);
    }
  }
}
