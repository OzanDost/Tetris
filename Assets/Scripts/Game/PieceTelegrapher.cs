using System;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class PieceTelegrapher : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Bounds _bounds;
        private Piece _currentPiece;
        private Transform _target;
        private float _lastRotation;
        private float _lastXPosition;

        private void Awake()
        {
            Signals.Get<CurrentPieceChanged>().AddListener(OnCurrentPieceChanged);
        }

        private void OnCurrentPieceChanged(Piece newPiece)
        {
            _currentPiece = newPiece;
            _target = newPiece.transform;
            // transform.SetParent(_target);
            // transform.localPosition = Vector3.zero;

            SetBounds(newPiece);
        }

        private void SetBounds(Piece piece)
        {
            _bounds = new Bounds(piece.transform.position, Vector3.zero);
            foreach (var pieceCollider in piece.Colliders)
            {
                // Bounds worldBounds = new Bounds(
                // piece.transform.TransformPoint(pieceCollider.bounds.center),
                // Vector3.Scale(pieceCollider.bounds.size, piece.transform.lossyScale)
                // );
                // _bounds.Encapsulate(worldBounds);
                _bounds.Encapsulate(pieceCollider.bounds);
            }

            // transform.localScale = new Vector3(_bounds.size.x, 100f, 1f);
            _spriteRenderer.size = new Vector2(_bounds.size.x, 100);
            transform.position = piece.transform.position;
            _bounds.center = transform.position;
        }


        private void OnDrawGizmos()
        {
            if (_target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);

                if (_currentPiece != null)
                {
                    Gizmos.color = Color.red;
                    foreach (var col in _currentPiece.Colliders)
                    {
                        Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (_currentPiece == null)
            {
                return;
            }

            _spriteRenderer.transform.position = _bounds.center;

            if (Math.Abs(_target.rotation.eulerAngles.z - _lastRotation) > 10)
            {
                _lastRotation = _target.rotation.eulerAngles.z;
                SetBounds(_currentPiece);
            }

            if (Mathf.Abs(_target.position.x - _lastXPosition) > 0.3f)
            {
                _lastXPosition = _target.position.x;
                SetBounds(_currentPiece);
            }
        }
    }
}