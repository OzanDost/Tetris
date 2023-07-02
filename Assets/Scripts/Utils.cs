using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static int LayerMaskToLayer(LayerMask layerMask)
    {
        return (int)Mathf.Log(layerMask, 2);
    }

    public static List<Vector2Int> GetOccupiedCellPositions(bool[,] matrix)
    {
        return Enumerable.Range(0, matrix.GetLength(0))
            .SelectMany(i => Enumerable.Range(0, matrix.GetLength(1))
                .Where(j => matrix[i, j])
                .Select(j => new Vector2Int(j, i)))
            .ToList();
    }

    public static Vector2Int GetPivotPosition(List<Vector2Int> positions, Vector2Int cellPosition)
    {
        var averagePosition = new Vector2Int((int)positions.Average(i => i.x), (int)positions.Average(i => i.y));
        return averagePosition - cellPosition;
    }
}