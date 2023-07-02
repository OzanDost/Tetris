using Enums;
using Game.Managers;
using ThirdParty;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class AIController : BoardController
    {
        private float _lastActionTime;
        private Vector2 _randomDirection;

        [SerializeField] private float _actionInterval = 1f;

        public override void Initialize()
        {
            gameObject.SetActive(true);
            base.Initialize();
        }

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

        protected override void PieceStateChanged(PieceState oldState, PieceState newState)
        {
            CurrentPiece.PieceStateChanged -= PieceStateChanged;

            if (oldState == PieceState.Active && newState == PieceState.Placed)
            {
                OnPiecePlaced();
                GetHeight();
            }
        }

        private Bounds _bounds;

        private float GetHeight()
        {
            if (!IsActive) return 0f;
            var isFirstPlacedPieceFound = false;
            var bounds = new Bounds();

            for (int i = 0; i < CurrentPieceIndex; i++)
            {
                if (Pieces[i].State != PieceState.Placed) continue;

                if (!isFirstPlacedPieceFound)
                {
                    bounds = new Bounds(Pieces[i].Colliders[0].bounds.center, Pieces[i].Colliders[0].bounds.size);
                    isFirstPlacedPieceFound = true;
                    continue;
                }

                foreach (var pieceCollider in Pieces[i].Colliders)
                {
                    bounds.Encapsulate(pieceCollider.bounds);
                }
            }

            _bounds.size = bounds.size;
            _bounds.center = bounds.center;

            Signals.Get<AIBoardHeightChanged>().Dispatch(bounds.size.y);

            return bounds.size.y;
        }

        protected override void ArrangeBoard(bool shouldSendEvent)
        {
            var groundPosition = Ground.transform.localPosition;
            FallZone.transform.localPosition = groundPosition - new Vector3(0, 5f, 0);
            StageFinishLine.SetLocalHeight(groundPosition.y + ConfigHelper.Config.WinLoseConditionConfig.TargetHeight);
            PieceSpawnPoint.localPosition = StageFinishLine.transform.localPosition + Vector3.up * 5f;

            if (shouldSendEvent)
            {
                Signals.Get<AIBoardArranged>().Dispatch(Ground.HorizontalBounds,
                    StageFinishLine.HorizontalBounds, PieceSpawnPoint);
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

            GetHeight();
            CheckMistakes(fallenPiece.State == PieceState.Placed);
            PoolManager.Instance.ReturnPiece(fallenPiece);
        }

        protected override void CheckMistakes(bool isPlacedPiece)
        {
            if (!IsActive) return;
            if (MistakeCount >= ConfigHelper.Config.WinLoseConditionConfig.AllowedMistakeCount)
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