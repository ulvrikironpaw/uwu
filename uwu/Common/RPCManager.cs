using System;
using System.Collections;
using UnityEngine;

namespace UWU.Common
{
  internal static class RPCManager
  {
    private const string SET_OBJECT_LABEL_NAME = "UWU_PerformRename";

    private static bool isRegistered = false;

    internal static void RegisterRPCs()
    {
      if (isRegistered) return;

      Jotunn.Logger.LogDebug($"Registering RPCs");
      ZRoutedRpc.instance.Register(
          SET_OBJECT_LABEL_NAME,
          new Action<long, ZPackage>(RPC_ServerReceiveRename));
      isRegistered = true;
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

      // If this is a dedicated server or any server at all, mutate directly.
      MutateZDO(target, CustomProperties.CUSTOM_LABEL_PROPERTY, label, ZNet.instance.IsServer());

      long serverPeer = ZRoutedRpc.instance.GetServerPeerID();
      if (serverPeer == 0)
      {
        Jotunn.Logger.LogWarning($"No server peer");
        return;
      }

      ZPackage pkg = new();
      pkg.Write(target.m_uid);
      pkg.Write(label);

      Jotunn.Logger.LogInfo($"Sending RPC: {SET_OBJECT_LABEL_NAME}");
      ZRoutedRpc.instance.InvokeRoutedRPC(serverPeer, SET_OBJECT_LABEL_NAME, pkg);
    }

    private static void RPC_ServerReceiveRename(long sender, ZPackage package)
    {
      var zdoid = package.ReadZDOID();
      var newName = package.ReadString();
      var zdo = ZDOMan.instance.GetZDO(zdoid);
      if (zdo == null) return;

      Jotunn.Logger.LogInfo($"Received RPC to {zdo.m_uid.UserID} {zdo.m_uid.ID}");
      MutateZDO(zdo, CustomProperties.CUSTOM_LABEL_PROPERTY, newName, false);
    }

    private static void MutateZDO(ZDO target, string property, string value, bool forceSync)
    {
      target.Set(property, value);
      if (forceSync)
      {
        ZDOMan.instance.ForceSendZDO(target.m_uid);
      }
    }
  }
}
