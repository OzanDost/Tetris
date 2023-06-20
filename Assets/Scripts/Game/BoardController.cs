using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Game.Managers;
using UnityEngine;

namespace Game
{
    public class BoardController : MonoBehaviour
    {
        private List<Piece> _pieces;
        private int _currentPieceIndex;
        private int _currentStageIndex;
        private bool _isActive;

        public Piece CurrentPiece => _pieces[_currentPieceIndex];
        private Piece _lastPieceTouchedFinishLine;
        private Vector2 _movementInput;

        [SerializeField] private Transform _pieceSpawnPoint;
        [SerializeField] private StageFinishLine _stageFinishLine;
        [SerializeField] private FallZone _fallZone;


        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _stageFinishLine.PieceReachedStageTarget += OnPieceReachedStageTarget;
            _fallZone.PieceFellOffBoard += OnPieceFellOffBoard;


            _pieces = new List<Piece>(250);
            _currentPieceIndex = 0;

            GeneratePieces();

            _isActive = true;

            ActivatePiece();
        }

        private void OnPieceReachedStageTarget(Collider2D pieceCollider)
        {
            if (BoardContainsCollider(pieceCollider, out Piece piece))
            {
                if (piece.State != PieceState.Placed) return;
                if (piece == _lastPieceTouchedFinishLine) return;
                _currentStageIndex++;
                _lastPieceTouchedFinishLine = piece;
                Debug.Log("Stage changed");
            }

            for (int i = 0; i < _currentPieceIndex; i++)
            {
                _pieces[i].Rigidbody2D.bodyType = RigidbodyType2D.Static;
                //todo add shiny effect here;
            }

            var defaultStageHeight = ConfigHelper.Config.defaultStageHeight;
            _stageFinishLine.IncreaseHeight(defaultStageHeight);
            _pieceSpawnPoint.position = _stageFinishLine.transform.position + Vector3.up * defaultStageHeight;
        }

        private void OnPieceFellOffBoard(Collider2D pieceCollider)
        {
            if (BoardContainsCollider(pieceCollider, out Piece piece))
            {
                //todo add mistake
            }
        }

        private bool BoardContainsCollider(Collider2D pieceCollider, out Piece parentPiece)
        {
            for (int i = 0; i < _currentPieceIndex; i++)
            {
                var piece = _pieces[i];
                if (piece.Colliders.Contains(pieceCollider))
                {
                    parentPiece = piece;
                    return true;
                }
            }

            parentPiece = null;
            return false;
        }

        private void ActivatePiece()
        {
            CurrentPiece.transform.position = _pieceSpawnPoint.position;
            CurrentPiece.Activate();
            CurrentPiece.OnPieceStateChanged += OnPieceStateChanged;
        }

        private void OnPieceStateChanged(PieceState oldState, PieceState newState)
        {
            CurrentPiece.OnPieceStateChanged -= OnPieceStateChanged;
            if (oldState == PieceState.Active && newState == PieceState.Placed)
            {
                OnPiecePlaced();
            }
        }

        private void OnPiecePlaced()
        {
            if (_currentPieceIndex + 1 >= _pieces.Count)
            {
                GeneratePieces(50);
            }

            _currentPieceIndex++;

            ActivatePiece();
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
            _movementInput.y = isSpeedy ? -1.5f : -0.5f;
        }

        private void FixedUpdate()
        {
            if (!_isActive) return;
            var piecePosition = CurrentPiece.Rigidbody2D.position;

            // Calculate new position based on integer increments
            int xMovement = Mathf.RoundToInt(_movementInput.x);
            float yMovement = piecePosition.y + _movementInput.y * 5f * Time.deltaTime;
            Vector2 newPosition = new Vector2(piecePosition.x + xMovement,
                yMovement);

            CurrentPiece.Rigidbody2D.MovePosition(newPosition);
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