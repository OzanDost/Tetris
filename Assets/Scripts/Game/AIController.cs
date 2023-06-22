using Enums;
using Game.Managers;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class AIController : BoardController
    {
        private float _lastActionTime;
        private Vector2 _randomDirection;

        [SerializeField] private float _actionInterval = 1f;

        protected override void SubscribeToEvents()
        {
            StageFinishLine.PieceReachedStageTarget += OnPieceReachedStageTarget;
            FallZone.PieceFellOffBoard += OnPieceFellOffBoard;

            Signals.Get<LevelQuit>().AddListener(OnLevelQuit);
        }

        protected override void UnsubscribeFromEvents()
        {
            StageFinishLine.PieceReachedStageTarget -= OnPieceReachedStageTarget;
            FallZone.PieceFellOffBoard -= OnPieceFellOffBoard;

            Signals.Get<LevelQuit>().RemoveListener(OnLevelQuit);
        }

        private void Update()
        {
            if (!IsActive || IsPaused) return;

            if (Time.time - _lastActionTime > _actionInterval)
            {
                _lastActionTime = Time.time;

                // Randomly decide whether to move horizontally or rotate
                if (Random.value > 0.5f)
                {
                    _randomDirection.x = Mathf.Round(Random.Range(-1f, 1f) * 2) / 2f;
                    MovePieceHorizontally(_randomDirection.x);
                }
                else
                {
                    RotatePiece();
                }

                // Randomly apply vertical fast move speed
                ToggleVerticalSpeed(Random.value > 0.6f);
            }
        }

        protected override void OnPieceFellOffBoard(Collider2D pieceCollider)
        {
            var fallenPiece = GetPieceFromCollider(pieceCollider, out bool foundPiece);
            if (!foundPiece) return;
            if (fallenPiece.State == PieceState.Inactive) return;
            if (fallenPiece == LastFallenPiece) return;
            LastFallenPiece = fallenPiece;

            MistakeCount++;

            CheckMistakes(fallenPiece.State == PieceState.Placed);
            PoolManager.Instance.ReturnPiece(fallenPiece);
        }

        protected override void CheckMistakes(bool isPlacedPiece)
        {
            if (MistakeCount >= ConfigHelper.Config.AllowedMistakeCount)
            {
                Signals.Get<AIMistakesFilled>().Dispatch();
                UnsubscribeFromEvents();
                IsActive = false;
                return;
            }

            if (!isPlacedPiece)
            {
                ActivatePiece();
            }
        }

        protected override void ActivatePiece()
        {
            // if (!IsActive || IsPaused) return;
            
            CurrentPiece = Pieces[CurrentPieceIndex];
            CurrentPiece.transform.position = PieceSpawnPoint.position;
            CurrentPiece.Activate();
            CurrentPiece.PieceStateChanged += PieceStateChanged;
            CurrentPieceIndex++;

            Signals.Get<AiPieceChanged>().Dispatch(CurrentPiece);
        }
    }
}