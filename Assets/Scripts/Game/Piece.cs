using System;
using DefaultNamespace;
using Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Piece : MonoBehaviour, IPoolableItem
    {
        public PieceState State => _state;
        public PieceType PieceType => _pieceType;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public Collider2D[] Colliders => _colliders;


        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Collider2D[] _colliders;
        [SerializeField] private SpriteRenderer[] _spriteRenderers;
        [SerializeField] private PieceType _pieceType;
        [SerializeField] private LayerMask _defaultLayerMask;
        [SerializeField] private LayerMask _placedLayerMask;

        private PieceState _state;

        public event Action<PieceState, PieceState> OnPieceStateChanged;

        public bool IsInPool { get; set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ToggleColliderTriggers(false);
            ChangeState(PieceState.Placed);
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            _rigidbody2D.gravityScale = 1f;
        }

        private void ChangeState(PieceState newState)
        {
            var oldState = _state;
            _state = newState;
            OnPieceStateChanged?.Invoke(oldState, newState);

            if (newState == PieceState.Placed)
            {
                ToggleLayer(true);
            }
        }

        [Button]
        public void Test()
        {
            ToggleLayer(true);
        }

        public void OnSpawn()
        {
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            ToggleColliderTriggers(true);
            ChangeState(PieceState.Active);
        }

        private void ToggleLayer(bool isPlaced)
        {
            int targetLayer = isPlaced ? _placedLayerMask.value : _defaultLayerMask.value;
            targetLayer = Utils.LayerMaskToLayer(targetLayer);
            gameObject.layer = targetLayer;
            foreach (var pieceCollider in _colliders)
            {
                pieceCollider.gameObject.layer = targetLayer;
            }
        }

        public void OnReturnToPool()
        {
            ToggleColliderTriggers(true);
            ChangeState(PieceState.Inactive);
            gameObject.SetActive(false);
            ToggleLayer(false);
        }

        public void Move(Vector2 direction, float speed)
        {
            if (_state != PieceState.Active) return;
            _rigidbody2D.MovePosition(_rigidbody2D.position + direction);
        }

        public void Rotate()
        {
            if (_state != PieceState.Active) return;

            transform.Rotate(0, 0, 90);
        }

        private void ToggleColliderTriggers(bool value)
        {
            foreach (var collider in _colliders)
            {
                collider.isTrigger = value;
            }
        }

#if UNITY_EDITOR

        public void SetReferencesFromEditor(Collider2D[] colliders, PieceType pieceType)
        {
            this._colliders = colliders;
            this._pieceType = pieceType;
        }
#endif
    }
}