using System.Collections.Generic;
using Enums;
using Game.Managers;
using UnityEngine;

namespace Game
{
    public class BoardController : MonoBehaviour
    {
        private List<Piece> _pieces;
        private int _currentPieceIndex;
        private bool _isActive;

        public Piece CurrentPiece => _pieces[_currentPieceIndex];
        private Vector2 _movementInput;


        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _pieces = new List<Piece>(250);
            _currentPieceIndex = 0;
            GeneratePieces();
            _isActive = true;

            ActivatePiece();
        }

        private void ActivatePiece()
        {
            CurrentPiece.Activate();
            CurrentPiece.OnPieceStateChanged += OnPieceStateChanged;
        }

        private void OnPieceStateChanged(PieceState oldState, PieceState newState)
        {
            CurrentPiece.OnPieceStateChanged -= OnPieceStateChanged;
            if (oldState == PieceState.Active && newState == PieceState.Placed)
            {
                _currentPieceIndex++;
                //todo add new pieces if needed
                ActivatePiece();
            }
        }

        public void ResetBoard()
        {
            _isActive = false;
            //todo make a different thing for saving
            foreach (var piece in _pieces)
            {
                PoolManager.Instance.ReturnPiece(piece);
            }
        }

        public void MovePieceHorizontally(Vector2 direction)
        {
            if (!_isActive) return;
            _movementInput.x = direction.normalized.x;
        }

        public void RotatePiece()
        {
            if (!_isActive) return;
            CurrentPiece.Rotate();
        }

        public void ToggleVerticalSpeed(bool isSpeedy)
        {
            _movementInput.y = isSpeedy ? -0.3f : -0.1f;
        }

        private void FixedUpdate()
        {
            if (!_isActive) return;
            var piecePosition = CurrentPiece.Rigidbody2D.position;
            CurrentPiece.Rigidbody2D.MovePosition(piecePosition + _movementInput * (4f * Time.fixedDeltaTime));
            _movementInput.x = 0f;
            _movementInput.y = -0.1f;
        }

        private void GeneratePieces(int amount = -1)
        {
            int piecesToGenerate = amount == -1 ? 250 : amount;
            for (int i = 0; i < piecesToGenerate; i++)
            {
                var piece = GetRandomPiece();
                _pieces.Add(piece);
            }
        }


        private Piece GetRandomPiece()
        {
            var randomPieceType = ConfigHelper.GetRandomPieceType();
            return PoolManager.Instance.GetPiece(randomPieceType);
        }
    }
}