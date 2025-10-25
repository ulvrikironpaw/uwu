using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UWU.Common
{
  internal class IconUtils
  {
    private static readonly Dictionary<string, Sprite> iconCache = new();

    /// <summary>
    /// Returns the Sprite for the icon if it exists in the build menu.
    /// </summary>
    /// <param name="shipPieceName"></param>
    /// <returns></returns>
    internal static Sprite GetBuildIconFromString(string pieceName)
    {
      if (iconCache.TryGetValue(pieceName, out var icon))
      {
        return icon;
      }
      // Search the piece table for a part that has the name.
      var pieceTable = PieceManager.Instance.GetPieceTable("Hammer");
      var shipPiece = pieceTable?.m_pieces.FirstOrDefault(p => p.name.Contains(pieceName));
      if (!shipPiece)
      {
        Jotunn.Logger.LogError($"Could not find piece with name containing '{pieceName}'");
        return null;
      }
      // Get the Piece component and return its icon if present.
      var piece = shipPiece.GetComponent<Piece>();
      icon = piece?.m_icon;
      iconCache[pieceName] = icon;
      return icon;
    }

    /// <summary>
    /// Returns the build icon if any from the ZDO's prefab definiton.
    /// </summary>
    /// <param name="zdo"></param>
    /// <returns></returns>
    internal static Sprite GetBuildIconFromZDO(ZDO zdo)
    {
      var prefabName = NameCache.GetNameFromZDO(zdo);
      if (string.IsNullOrWhiteSpace(prefabName)) return null;
      prefabName = prefabName.Replace("(Clone)", "").Trim();
      return GetBuildIconFromString(prefabName);
    }
  }
}
