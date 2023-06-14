using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Data;
using Enums;
using UnityEngine;

namespace Game.Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }
        [SerializeField] private PoolConfig _poolConfig;

        Dictionary<PieceType, List<IPoolableItem>> _itemDictionary;

        private void Awake()
        {
            Instance = this;

            Initialize();
        }

        private void Initialize()
        {
            _itemDictionary = new Dictionary<PieceType, List<IPoolableItem>>();

            foreach (var item in _poolConfig.Items)
            {
                var list = new List<IPoolableItem>();
                for (int i = 0; i < item.InitialGenerationCount; i++)
                {
                    var piece = Instantiate(item.Piece, transform);
                    piece.gameObject.SetActive(false);
                    piece.IsInPool = true;
                    list.Add(piece);
                }

                _itemDictionary.Add(item.Piece.PieceType, list);
            }
        }

        public Piece GetPiece(PieceType pieceType)
        {
            var list = _itemDictionary[pieceType];
            foreach (var item in list)
            {
                if (item.IsInPool)
                {
                    item.IsInPool = false;
                    item.OnSpawn();
                    return item as Piece;
                }
            }

            //todo maybe refine here
            var piece = Instantiate(_poolConfig.Items.Find(x => x.Piece.PieceType == pieceType).Piece, transform);
            piece.IsInPool = false;
            piece.OnSpawn();
            return piece;
        }

        public void ReturnPiece(Piece piece)
        {
            piece.IsInPool = true;
            piece.OnReturnToPool();
        }
    }
}