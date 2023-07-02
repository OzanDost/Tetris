using System;
using Game;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PoolableItemData
    {
        public Piece Piece;
        public int InitialGenerationCount;
        private GameObject Prefab => Piece.gameObject;

        public PoolableItemData(Piece piece, int initialGenerationCount)
        {
            Piece = piece;
            InitialGenerationCount = initialGenerationCount;
        }
    }


   
}