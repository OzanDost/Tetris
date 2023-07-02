using Data;
using Game;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PoolAssetModificationHandler : AssetModificationProcessor
    {
        private const string PoolConfigFilePath = "PoolConfig";


        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var poolConfig = Resources.Load<PoolConfig>(PoolConfigFilePath);
            var item = AssetDatabase.LoadAssetAtPath<Piece>(assetPath);
            if (item == null) return AssetDeleteResult.DidNotDelete;

            bool Data(PoolableItemData itemData) => itemData.Piece == item;

            if (poolConfig.Items.Exists(Data))
            {
                poolConfig.Items.Remove(poolConfig.Items.Find(itemData => itemData.Piece == item));
                EditorUtility.SetDirty(poolConfig);
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}