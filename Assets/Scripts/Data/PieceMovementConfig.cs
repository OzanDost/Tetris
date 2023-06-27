using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PieceMovementConfig
    {
        public float HorizontalMoveSpeed => horizontalMoveSpeed;

        [BoxGroup("Piece Movement Configs")]
        [PropertyRange(3, 10)]
        [SerializeField] private float horizontalMoveSpeed;


        public float HorizontalMoveStep => _horizontalMoveStep;
        [BoxGroup("Piece Movement Configs")]
        [SerializeField] private float _horizontalMoveStep;

        public float VerticalMoveSpeed => _verticalMoveSpeed;

        [BoxGroup("Piece Movement Configs")]
        [PropertyRange(0.5, 2)]
        [SerializeField] private float _verticalMoveSpeed;

        public float VerticalFastMoveSpeed => _verticalFastMoveSpeed;

        [BoxGroup("Piece Movement Configs")]
        [PropertyRange(1.5, 5)]
        [SerializeField] private float _verticalFastMoveSpeed;

    }
}