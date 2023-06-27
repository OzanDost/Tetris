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

        private Piece _lastPieceTouchedFinishLine;
        private Vector2 _movementInput;
        private GameConfig _gameConfig;
        private Tween _freezeDelayTween;
        private Bounds _pieceMovementBounds;

        protected Piece CurrentPiece { get; set; }

        [SerializeField] protected Transform PieceSpawnPoint;
        [SerializeField] protected StageFinishLine StageFinishLine;
        [SerializeField] protected FallZone FallZone;
        [SerializeField] private BoxCollider2D _pieceConfiner;
        [SerializeField] private Ground _ground;


        public void Initialize()
        {
            SubscribeToEvents();

            Pieces = new List<Piece>(50);
            CurrentPieceIndex = 0;
            _gameConfig = ConfigHelper.Config;

            GeneratePieces();
            ArrangeBoard(true);
            ActivatePiece();

            IsPaused = false;
            IsActive = true;
            _pieceMovementBounds = _pieceConfiner.bounds;
        }

        protected virtual void SubscribeToEvents()
        {
            StageFinishLine.PieceReachedStageTarget += OnPieceReachedStageTarget;
            FallZone.PieceFellOffBoard += OnPieceFellOffBoard;

            Signals.Get<TogglePause>().AddListener(OnPauseToggled);
            Signals.Get<LevelQuit>().AddListener(OnLevelQuit);
            Signals.Get<RotateInputGiven>().AddListener(RotatePiece);
            Signals.Get<HorizontalInputGiven>().AddListener(MovePieceHorizontally);
            Signals.Get<VerticalSpeedToggled>().AddListener(ToggleVerticalSpeed);
        }

        protected virtual void UnsubscribeFromEvents()
        {
            StageFinishLine.PieceReachedStageTarget -= OnPieceReachedStageTarget;
            FallZone.PieceFellOffBoard -= OnPieceFellOffBoard;

            Signals.Get<TogglePause>().RemoveListener(OnPauseToggled);
            Signals.Get<LevelQuit>().RemoveListener(OnLevelQuit);
            Signals.Get<RotateInputGiven>().RemoveListener(RotatePiece);
            Signals.Get<HorizontalInputGiven>().RemoveListener(MovePieceHorizontally);
            Signals.Get<VerticalSpeedToggled>().RemoveListener(ToggleVerticalSpeed);
        }

        private void OnPauseToggled(bool isPaused)
        {
            IsPaused = isPaused;
            Time.timeScale = isPaused ? 0 : 1;
        }

        protected void OnLevelQuit()
        {
            IsActive = false;
            ResetBoard();
            UnsubscribeFromEvents();
        }

        private void ArrangeBoard(bool shouldSendEvent)
        {
            var groundPosition = _ground.transform.localPosition;
            FallZone.transform.localPosition = groundPosition - new Vector3(0, 5f, 0);
            StageFinishLine.SetLocalHeight(groundPosition.y + ConfigHelper.Config.WinLoseConditionConfig.TargetHeight);
            PieceSpawnPoint.localPosition = StageFinishLine.transform.localPosition + Vector3.up * 5f;

            if (shouldSendEvent)
            {
                Signals.Get<BoardArranged>().Dispatch(_ground.HorizontalBounds,
                    StageFinishLine.HorizontalBounds, PieceSpawnPoint);
            }
        }

        protected void OnPieceReachedStageTarget(Collider2D pieceCollider)
        {
            var reachedPiece = GetPieceFromCollider(pieceCollider, out bool foundPiece);
            if (!foundPiece) return;
            if (reachedPiece.State != PieceState.Placed) return;
            if (reachedPiece == _lastPieceTouchedFinishLine) return;

            _lastPieceTouchedFinishLine = reachedPiece;

            CalculateHeight();
            Signals.Get<LevelFinished>().Dispatch(true);

            OnLevelFinished(true);
        }

        protected virtual void OnPieceFellOffBoard(Collider2D pieceCollider)
        {
            var fallenPiece = GetPieceFromCollider(pieceCollider, out bool foundPiece);
            if (!foundPiece) return;
            if (fallenPiece.State == PieceState.Inactive) return;
            if (fallenPiece == LastFallenPiece) return;
            LastFallenPiece = fallenPiece;

            MistakeCount++;
            fallenPiece.PieceStateChanged -= PieceStateChanged;

            Signals.Get<LifeLost>().Dispatch();

            CheckMistakes(fallenPiece.State == PieceState.Placed);

            PoolManager.Instance.ReturnPiece(fallenPiece);
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
            if (MistakeCount >= ConfigHelper.Config.WinLoseConditionConfig.AllowedMistakeCount)
            {
                OnLevelFinished(false);
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
            Signals.Get<NextPieceChanged>().Dispatch(Pieces[CurrentPieceIndex]);
        }

        private void FreezePlacedPieces()
        {
            _freezeDelayTween = DOVirtual.DelayedCall(0.3f, () =>
            {
                for (int i = 0; i < CurrentPieceIndex - 1; i++)
                {
                    if (Pieces[i].State == PieceState.Inactive) continue;
                    Pieces[i].SetRigidbodyMode(RigidbodyType2D.Static);
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
            if (!IsActive) return;
            if (CurrentPieceIndex + 1 >= Pieces.Count)
            {
                GeneratePieces(50);
            }

            Signals.Get<PiecePlaced>().Dispatch(CurrentPiece);

            ActivatePiece();
        }

        private void OnLevelFinished(bool isSuccess)
        {
            IsActive = false;
            FreezePlacedPieces();
            PoolManager.Instance.ReturnPiece(CurrentPiece);
            Signals.Get<LevelFinished>().Dispatch(isSuccess);
        }

        private void CalculateHeight()
        {
            var bounds = new Bounds();
            for (int i = 0; i < CurrentPieceIndex; i++)
            {
                if (Pieces[i].State != PieceState.Placed) continue;
                foreach (var pieceCollider in Pieces[i].Colliders)
                {
                    bounds.Encapsulate(pieceCollider.bounds);
                }
            }

            Signals.Get<BoardHeightCalculated>().Dispatch(bounds.size.y);
        }

        public float GetWidth()
        {
            var bounds = new Bounds();
            foreach (var point in StageFinishLine.HorizontalBounds)
            {
                bounds.Encapsulate(point.position);
            }

            return bounds.size.x;
        }

        public void ResetBoard()
        {
            IsActive = false;
            foreach (var piece in Pieces)
            {
                CurrentPiece.PieceStateChanged -= PieceStateChanged;
                PoolManager.Instance.ReturnPiece(piece);
            }

            ArrangeBoard(false);
        }

        protected void MovePieceHorizontally(float direction)
        {
            if (!IsActive || IsPaused) return;
            _movementInput.x = direction > 0f
                ? _gameConfig.PieceMovementConfig.HorizontalMoveStep
                : -_gameConfig.PieceMovementConfig.HorizontalMoveStep;
        }

        protected void RotatePiece()
        {
            if (!IsActive || IsPaused) return;
            CurrentPiece.Rotate();
        }

        protected void ToggleVerticalSpeed(bool isSpeedy)
        {
            _movementInput.y = isSpeedy
                ? -_gameConfig.PieceMovementConfig.VerticalFastMoveSpeed
                : -_gameConfig.PieceMovementConfig.VerticalMoveSpeed;
        }

        private void FixedUpdate()
        {
            if (!IsActive || IsPaused) return;
            if (CurrentPiece == null) return;
            var piecePosition = CurrentPiece.Rigidbody2D.position;

            // Calculate new position based on integer increments
            float xMovement = piecePosition.x + _movementInput.x;
            float yMovement = piecePosition.y +
                              _movementInput.y * _gameConfig.PieceMovementConfig.HorizontalMoveSpeed * Time.deltaTime;
            Vector2 newPosition = new Vector2(xMovement, yMovement);

            newPosition.x = Mathf.Clamp(newPosition.x, _pieceMovementBounds.min.x + CurrentPiece.PieceBounds.extents.x,
                _pieceMovementBounds.max.x - CurrentPiece.PieceBounds.extents.x);

            CurrentPiece.Rigidbody2D.MovePosition(newPosition);

            _movementInput.x = 0f;
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