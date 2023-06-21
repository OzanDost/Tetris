using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Game.Managers;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class BoardController : MonoBehaviour
    {
        private List<Piece> _pieces;
        private int _currentPieceIndex;
        private int _currentStageIndex;
        private bool _isActive;
        private bool _isPaused;
        private Piece _lastPieceTouchedFinishLine;
        private Piece _lastFallenPiece;
        private Vector2 _movementInput;
        private int _mistakeCount;
        private GameConfig _gameConfig;

        private Piece CurrentPiece { get; set; }

        [SerializeField] private Transform _pieceSpawnPoint;
        [SerializeField] private StageFinishLine _stageFinishLine;
        [SerializeField] private FallZone _fallZone;
        [SerializeField] private Ground _ground;


        public void Initialize()
        {
            SubscribeToEvents();

            _pieces = new List<Piece>(250);
            _currentPieceIndex = 0;
            _gameConfig = ConfigHelper.Config;

            GeneratePieces();
            ArrangeBoard();
            ActivatePiece();

            _isActive = true;
        }

        private void SubscribeToEvents()
        {
            _stageFinishLine.PieceReachedStageTarget += OnPieceReachedStageTarget;
            _fallZone.PieceFellOffBoard += OnPieceFellOffBoard;

            Signals.Get<PauseRequested>().AddListener(TogglePause);
            Signals.Get<PauseCanceled>().AddListener(TogglePause);
            Signals.Get<LevelQuit>().AddListener(OnLevelQuit);
        }

        private void OnLevelQuit()
        {
            _isActive = false;
            ResetBoard();
        }

        private void TogglePause()
        {
            _isPaused = !_isPaused;

            Time.timeScale = _isPaused ? 0 : 1;
        }

        private void ArrangeBoard()
        {
            _ground.transform.localPosition = Vector3.zero;
            _fallZone.transform.localPosition = new Vector3(0, -5f, 0);
            _stageFinishLine.SetLocalHeight(ConfigHelper.Config.DefaultStageHeight); //todo
            _pieceSpawnPoint.localPosition = _stageFinishLine.transform.localPosition + Vector3.up * 5f;

            Signals.Get<BoardArranged>().Dispatch(_ground.HorizontalBounds, _pieceSpawnPoint);
        }

        private void OnPieceReachedStageTarget(Collider2D pieceCollider)
        {
            var reachedPiece = GetPieceFromCollider(pieceCollider);
            if (reachedPiece.State != PieceState.Placed) return;
            if (reachedPiece == _lastPieceTouchedFinishLine) return;

            _currentStageIndex++;
            _lastPieceTouchedFinishLine = reachedPiece;

            //do this inside an coroutine
            for (int i = 0; i < _currentPieceIndex - 1; i++)
            {
                if (_pieces[i].State != PieceState.Placed) continue;
                _pieces[i].SetRigidbodyMode(RigidbodyType2D.Static);
                //todo add shiny effect here;
            }

            var defaultStageHeight = ConfigHelper.Config.DefaultStageHeight;
            _stageFinishLine.IncreaseHeight(defaultStageHeight);
            _pieceSpawnPoint.position = _stageFinishLine.transform.position + Vector3.up * defaultStageHeight;
        }

        private void OnPieceFellOffBoard(Collider2D pieceCollider)
        {
            var fallenPiece = GetPieceFromCollider(pieceCollider);
            if (fallenPiece.State == PieceState.Inactive) return;
            if (fallenPiece == _lastFallenPiece) return;
            _lastFallenPiece = fallenPiece;

            _mistakeCount++;

            Signals.Get<LifeLost>().Dispatch();
            CheckMistakes(fallenPiece.State == PieceState.Placed);
            PoolManager.Instance.ReturnPiece(fallenPiece);

            Debug.Log($"Piece{fallenPiece.name} Fell off board. Mistake Count: {_mistakeCount}");
        }

        private Piece GetPieceFromCollider(Collider2D pieceCollider)
        {
            Piece piece = null;
            for (int i = 0; i < _currentPieceIndex; i++)
            {
                if (_pieces[i].Colliders.Contains(pieceCollider))
                {
                    piece = _pieces[i];
                }
            }

            return piece;
        }

        private void CheckMistakes(bool isPlacedPiece)
        {
            if (_mistakeCount >= ConfigHelper.Config.AllowedMistakeCount)
            {
                Signals.Get<LevelFinished>().Dispatch();
                return;
            }

            if (!isPlacedPiece)
            {
                ActivatePiece();
            }
        }

        private void ActivatePiece()
        {
            CurrentPiece = _pieces[_currentPieceIndex];
            CurrentPiece.transform.position = _pieceSpawnPoint.position;
            CurrentPiece.Activate();
            CurrentPiece.PieceStateChanged += PieceStateChanged;
            _currentPieceIndex++;
            Signals.Get<CurrentPieceChanged>().Dispatch(CurrentPiece);
        }

        private void PieceStateChanged(PieceState oldState, PieceState newState)
        {
            CurrentPiece.PieceStateChanged -= PieceStateChanged;

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

            Debug.Log("Piece Placed");
            ActivatePiece();
        }


        public void ResetBoard()
        {
            _isActive = false;
            //todo make a different thing for saving
            foreach (var piece in _pieces)
            {
                CurrentPiece.PieceStateChanged -= PieceStateChanged;
                PoolManager.Instance.ReturnPiece(piece);
            }
        }

        public void MovePieceHorizontally(Vector2 direction)
        {
            if (!_isActive || _isPaused) return;
            _movementInput.x = direction.normalized.x / 2f;
        }

        public void RotatePiece()
        {
            if (!_isActive || _isPaused) return;
            CurrentPiece.Rotate();
        }

        public void ToggleVerticalSpeed(bool isSpeedy)
        {
            _movementInput.y = isSpeedy ? -_gameConfig.VerticalFastMoveSpeed : -_gameConfig.VerticalMoveSpeed;
        }

        private void FixedUpdate()
        {
            if (!_isActive || _isPaused) return;
            var piecePosition = CurrentPiece.Rigidbody2D.position;

            // Calculate new position based on integer increments
            float xMovement = piecePosition.x + _movementInput.x;
            // (float)Math.Round((piecePosition.x + _movementInput.x) * 2f, MidpointRounding.AwayFromZero) / 2f;
            float yMovement = piecePosition.y + _movementInput.y * _gameConfig.HorizontalMoveSpeed * Time.deltaTime;
            Vector2 newPosition = new Vector2(xMovement, yMovement);

            CurrentPiece.Rigidbody2D.MovePosition(newPosition);
            _movementInput.x = 0;
            _movementInput.y = -_gameConfig.VerticalMoveSpeed;
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