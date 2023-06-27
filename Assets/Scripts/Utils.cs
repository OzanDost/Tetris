using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEditor;
using UnityEngine;

public static class Utils
{
    public static int LayerMaskToLayer(LayerMask layerMask)
    {
        return (int)Mathf.Log(layerMask, 2);
    }

    public static IEnumerable<Piece> GetSavedPieces()
    {
        var existingPieces = AssetDatabase.FindAssets($"t:Prefab Piece_")
            .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(piece => piece != null)
            .Select(piece => piece.GetComponent<Piece>())
            .Where(pieceComponent => pieceComponent != null);

        return existingPieces;
    }
}