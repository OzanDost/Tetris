using System;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PoolableItemData
    {
        [VerticalGroup("row1/left")]
        public Piece Piece;

        [VerticalGroup("row1/left")]
        public int InitialGenerationCount;

        [HorizontalGroup("row1", 80), VerticalGroup("row1/right")]
        [PreviewField(80, ObjectFieldAlignment.Right)]
        [ShowInInspector]
        [HideLabel]
        private GameObject Prefab => Piece.gameObject;

        public PoolableItemData(Piece piece, int initialGenerationCount)
        {
            Piece = piece;
            InitialGenerationCount = initialGenerationCount;
        }
    }
}