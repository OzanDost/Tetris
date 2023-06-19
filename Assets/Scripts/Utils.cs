using UnityEngine;

public static class Utils
{
    public static int LayerMaskToLayer(LayerMask layerMask)
    {
        return (int)Mathf.Log(layerMask, 2);
    }
}