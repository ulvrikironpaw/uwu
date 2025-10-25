using HarmonyLib;
using System.Collections.Generic;

namespace UWU.Extensions
{
  /// <summary>
  /// Provides extension methods for ZNetScene
  /// </summary>
  internal static class ZNetSceneExtensions
  {
    private static readonly AccessTools.FieldRef<ZNetScene, Dictionary<ZDO, ZNetView>> Instances =
      AccessTools.FieldRefAccess<ZNetScene, Dictionary<ZDO, ZNetView>>("m_instances");

    public static Dictionary<ZDO, ZNetView> GetInstances_UWU(this ZNetScene zNetScene)
    {
      return Instances(zNetScene);
    }
  }
}
