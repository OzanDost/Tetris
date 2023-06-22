using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using Enums;
using Game.Managers;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class BoardController : MonoBehaviour
    {
        protected bool IsActive;
        protected bool IsPaused;
        protected int MistakeCount;
        protected int CurrentPieceIndex;
        protected List<Piece> Pieces;
        protected Piece LastFallenPiece;

        private int _currentStageIndex;
        private Piece _lastPieceTouchedFinishLine;
        private Vector2 _movementInput;
        private GameConfig _gameConfig;
        private Tween _freezeDelayTween;

        protected Piece CurrentPiece { get; set; }

        [SerializeField] protected Transform PieceSpawnPoint;
        [SerializeField] protected StageFinishLine StageFinishLine;
        [SerializeField] protected FallZone FallZone;
        
        [SerializeField] private Ground _ground;


        public void Initialize()
        {
            SubscribeToEvents();

            Pieces = new List<Piece>(250);
            CurrentPieceIndex = 0;
            _gameConfig = ConfigHelper.Config;

            GeneratePieces();
            ArrangeBoard();
            ActivatePiece();

            IsActive = true;
        }

        protected virtual void SubscribeToEvents()
        {
            StageFinishLine.PieceReachedStageTarget += OnPieceReachedStageTarget;
            FallZone.PieceFellOffBoard += OnPieceFellOffBoard;

            Signals.Get<PauseRequested>().AddListener(TogglePause);
            Signals.Get<PauseCanceled>().AddListener(TogglePause);
            Signals.Get<LivesFinished>().AddListener(OnLivesFinished);
            Signals.Get<LevelQuit>().AddListener(OnLevelQuit);
            Signals.Get<RotateInputGiven>().AddListener(RotatePiece);
            Signals.Get<HorizontalInputGiven>().AddListener(MovePieceHorizontally);
            Signals.Get<VerticalSpeedToggled>().AddListener(ToggleVerticalSpeed);
        }

        protected virtual void UnsubscribeFromEvents()
        {
            StageFinishLine.PieceReachedStageTarget -= OnPieceReachedStageTarget;
            FallZone.PieceFellOffBoard -= OnPieceFellOffBoard;

            Signals.Get<PauseRequested>().RemoveListener(TogglePause);
            Signals.Get<PauseCanceled>().RemoveListener(TogglePause);
            Signals.Get<LivesFinished>().RemoveListener(OnLivesFinished);
            Signals.Get<LevelQuit>().RemoveListener(OnLevelQuit);
            Signals.Get<RotateInputGiven>().RemoveListener(RotatePiece);
            Signals.Get<HorizontalInputGiven>().RemoveListener(MovePieceHorizontally);
            Signals.Get<VerticalSpeedToggled>().RemoveListener(ToggleVerticalSpeed);
        }

        private void OnLivesFinished()
        {
            //todo bring in the finishing piece
        }

        protected void OnLevelQuit()
        {
            IsActive = false;
            ResetBoard();
            UnsubscribeFromEvents();
        }

        private void TogglePause()
        {
            IsPaused = !IsPaused;

            Time.timeScale = IsPaused ? 0 : 1;
        }

        private void ArrangeBoard()
        {
            _ground.transform.localPosition = Vector3.zero;
            FallZone.transform.localPosition = new Vector3(0, -5f, 0);
            StageFinishLine.SetLocalHeight(ConfigHelper.Config.DefaultStageHeight); //todo
            PieceSpawnPoint.localPosition = StageFinishLine.transform.localPosition + Vector3.up * 5f;

            Signals.Get<BoardArranged>().Dispatch(_ground.HorizontalBounds, PieceSpawnPoint);
        }

        protected void OnPieceReachedStageTarget(Collider2D pieceCollider)
        {
            var reachedPiece = GetPieceFromCollider(pieceCollider, out bool foundPiece);
            if (!foundPiece) return;
            if (reachedPiece.State != PieceState.Placed) return;
            if (reachedPiece == _lastPieceTouchedFinishLine) return;

            _currentStageIndex++;
            _lastPieceTouchedFinishLine = reachedPiece;

            FreezePlacedPieces();

            var defaultStageHeight = ConfigHelper.Config.DefaultStageHeight;
            StageFinishLine.IncreaseHeight(defaultStageHeight);
            PieceSpawnPoint.position = StageFinishLine.transform.position + Vector3.up * defaultStageHeight;
        }

        protected virtual void OnPieceFellOffBoard(Collider2D pieceCollider)
        {
            var fallenPiece = GetPieceFromCollider(pieceCollider, out bool foundPiece);
            if (!foundPiece) return;
            if (fallenPiece.State == PieceState.Inactive) return;
            if (fallenPiece == LastFallenPiece) return;
            LastFallenPiece = fallenPiece;

            MistakeCount++;

            Signals.Get<LifeLost>().Dispatch();
            CheckMistakes(fallenPiece.State == PieceState.Placed);
            PoolManager.Instance.ReturnPiece(fallenPiece);

            Debug.Log($"Piece{fallenPiece.name} Fell off board. Mistake Count: {MistakeCount}");
        }

        protected Piece GetPieceFromCollider(Collider2D pieceCollider, out bool foundPiece)
        {
            for (int i = 0; i < CurrentPieceIndex; i++)
            {
                if (Pieces[i].Colliders.Contains(pieceCollider))
                {
                    foundPiece = true;
                    return Pieces[i];
                }
            }

            foundPiece = false;
            return null;
        }

        protected virtual void CheckMistakes(bool isPlacedPiece)
        {
            if (MistakeCount >= ConfigHelper.Config.AllowedMistakeCount)
            {
                Signals.Get<LivesFinished>().Dispatch();
                return;
            }

            if (!isPlacedPiece)
            {
                ActivatePiece();
            }
        }

        protected virtual void ActivatePiece()
        {
            CurrentPiece = Pieces[CurrentPieceIndex];
            CurrentPiece.transform.position = PieceSpawnPoint.position;
            CurrentPiece.Activate();
            CurrentPiece.PieceStateChanged += PieceStateChanged;
            CurrentPieceIndex++;
            Signals.Get<CurrentPieceChanged>().Dispatch(CurrentPiece);
        }

        private void FreezePlacedPieces()
        {
            _freezeDelayTween = DOVirtual.DelayedCall(0.3f, () =>
            {
                for (int i = 0; i < CurrentPieceIndex - 1; i++)
                {
                    if (Pieces[i].State != PieceState.Placed) continue;
                    Pieces[i].SetRigidbodyMode(RigidbodyType2D.Static);
                    //todo add shiny effect here;
                }
            });
        }

        protected void PieceStateChanged(PieceState oldState, PieceState newState)
        {
            CurrentPiece.PieceStateChanged -= PieceStateChanged;

            if (oldState == PieceState.Active && newState == PieceState.Placed)
            {
                OnPiecePlaced();
            }
        }

        private void OnPiecePlaced()
        {
            if (CurrentPieceIndex + 1 >= Pieces.Count)
            {
                GeneratePieces(50);
            }

            Debug.Log("Piece Placed");
            ActivatePiece();
        }


        public void ResetBoard()
        {
            IsActive = false;
            //todo make a different thing for saving
            foreach (var piece in Pieces)
            {
                CurrentPiece.PieceStateChanged -= PieceStateChanged;
                PoolManager.Instance.ReturnPiece(piece);
            }

            ArrangeBoard();
        }

        protected void MovePieceHorizontally(float direction)
        {
            if (!IsActive || IsPaused) return;
            _movementInput.x = direction > 0 ? 0.5f : -0.5f;
        }

        protected void RotatePiece()
        {
            if (!IsActive || IsPaused) return;
            CurrentPiece.Rotate();
        }

        protected void ToggleVerticalSpeed(bool isSpeedy)
        {
            _movementInput.y = isSpeedy ? -_gameConfig.VerticalFastMoveSpeed : -_gameConfig.VerticalMoveSpeed;
        }

        private void FixedUpdate()
        {
            if (!IsActive || IsPaused) return;
            var piecePosition = CurrentPiece.Rigidbody2D.position;

            // Calculate new position based on integer increments
            float xMovement = piecePosition.x + _movementInput.x;
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
                Pieces.Add(piece);
            }
        }


        private Piece GetRandomPiece()
        {
            var randomPieceType = ConfigHelper.GetRandomPieceType();
            return PoolManager.Instance.GetPiece(randomPieceType);
        }
    }
}