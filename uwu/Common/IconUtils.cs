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
    /// Returns the 
    /// </summary>
    /// <param name="shipPieceName"></param>
    /// <returns></returns>
    public static Sprite GetBuildIconFromString(string pieceName)
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

    public static Sprite GetBuildIconFromZDO(ZDO zdo)
    {
      var prefabName = NameCache.GetNameFromZDO(zdo);
      if (string.IsNullOrWhiteSpace(prefabName)) return null;
      prefabName = prefabName.Replace("(Clone)", "").Trim();
      return GetBuildIconFromString(prefabName);
    }
  }
}
