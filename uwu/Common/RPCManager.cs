using System;
using System.Collections;
using UnityEngine;

namespace UWU.Common
{
  internal static class RPCManager
  {
    private const string SET_OBJECT_LABEL_NAME = "UWU_PerformRename";

    internal static IEnumerator RegisterRPCs()
    {
      while (ZRoutedRpc.instance == null) yield return null;

      Jotunn.Logger.LogDebug($"Registering RPCs");
      ZRoutedRpc.instance.Register(
          SET_OBJECT_LABEL_NAME,
          new Action<long, ZPackage>(RPC_ServerReceiveRename));
    }

    internal static void RenameObject(MonoBehaviour target, string label)
    {
      RenameObject(target.GetComponent<ZNetView>(), label);
    }

    internal static void RenameObject(ZNetView zNetView, string label)
    {
      if (zNetView == null) return;

      var zdo = zNetView.GetZDO();
      if (zdo == null) return;

      RenameObject(zdo, label);
    }

    internal static void RenameObject(ZDO target, string label)
    {
      if (target == null) return;
      ZPackage pkg = new();
      pkg.Write(target.m_uid);
      pkg.Write(label);

      Jotunn.Logger.LogDebug($"Sending RPC");
      ZRoutedRpc.instance.InvokeRoutedRPC(SET_OBJECT_LABEL_NAME, pkg);
    }

    private static void RPC_ServerReceiveRename(long sender, ZPackage package)
    {
      var zdoid = package.ReadZDOID();
      var newName = package.ReadString();
      var zdo = ZDOMan.instance.GetZDO(zdoid);
      if (zdo == null) return;

      Jotunn.Logger.LogDebug($"Received RPC to {zdo.m_uid.UserID} {zdo.m_uid.ID}");
      zdo.Set(Constants.CUSTOM_LABEL_PROPERTY, newName);
    }
  }
}
