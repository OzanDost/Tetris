using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PieceMovementConfig
    {
        public float HorizontalMoveSpeed => horizontalMoveSpeed;

        [SerializeField] private float horizontalMoveSpeed;


        public float HorizontalMoveStep => _horizontalMoveStep;
        [SerializeField] private float _horizontalMoveStep;

        public float VerticalMoveSpeed => _verticalMoveSpeed;

        [SerializeField] private float _verticalMoveSpeed;

        public float VerticalFastMoveSpeed => _verticalFastMoveSpeed;

        [SerializeField] private float _verticalFastMoveSpeed;

    }
}