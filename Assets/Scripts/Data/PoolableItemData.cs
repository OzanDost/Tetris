using System;
using Game;

namespace DefaultNamespace.Data
{
    [Serializable]
    public class PoolableItemData
    {
        public Piece Piece;
        public int InitialGenerationCount;
    }
}