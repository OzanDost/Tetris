using System.Collections.Generic;
using Data;
using DefaultNamespace.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PoolConfigEditor : OdinEditorWindow
    {
        private const string PoolConfigFilePath = "PoolConfig";

        [OnValueChanged("OnItemsChanged")]
        [SerializeField] private List<PoolableItemData> items;

        private PoolConfig _poolConfig;

        public void Init()
        {
            _poolConfig = Resources.Load<PoolConfig>(PoolConfigFilePath);
            items = _poolConfig.Items;

            OnClose -= OnClosed;
            OnClose += OnClosed;
        }

        private void OnClosed()
        {
            AssetDatabase.SaveAssetIfDirty(_poolConfig);
        }

        private void OnItemsChanged()
        {
            EditorUtility.SetDirty(_poolConfig);
        }
    }
}