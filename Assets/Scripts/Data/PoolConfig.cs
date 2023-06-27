using System.Collections.Generic;
using Data;
using UnityEngine;

namespace DefaultNamespace.Data
{
    [CreateAssetMenu(fileName = "Pool Config", menuName = "Custom/Pool Config", order = 0)]
    public class PoolConfig : ScriptableObject
    {
        public List<PoolableItemData> Items => items;
        [SerializeField]private List<PoolableItemData> items;
        
        
        public void AddItem(PoolableItemData item)
        {
            items.Add(item);
        }
    }
}