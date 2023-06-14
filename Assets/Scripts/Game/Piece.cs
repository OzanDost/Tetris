using System;
using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Game
{
    public class Piece : MonoBehaviour, IPoolableItem
    {
        public PieceState State => _state;
        public PieceType PieceType => _pieceType;

        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Collider2D[] _colliders;
        [SerializeField] private SpriteRenderer[] _spriteRenderers;
        [SerializeField] private PieceType _pieceType;

        private PieceState _state;

        public event Action<PieceState, PieceState> OnPieceStateChanged;

        public bool IsInPool { get; set; }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_state != PieceState.Active) return;


            //todo refine here
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            ToggleColliderTriggers(false);
            ChangeState(PieceState.Placed);
            _rigidbody2D.velocity = Vector2.zero;
        }

        private void ChangeState(PieceState newState)
        {
            var oldState = _state;
            _state = newState;
            OnPieceStateChanged?.Invoke(oldState, newState);
        }

        public void OnSpawn()
        {
            ToggleColliderTriggers(true);
            ChangeState(PieceState.Active);
        }

        public void OnReturnToPool()
        {
            ChangeState(PieceState.Inactive);
        }

        public void Move(Vector2 direction)
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