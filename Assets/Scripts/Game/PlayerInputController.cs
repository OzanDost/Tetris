using Enums;
using Game.Managers;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class PlayerInputController : MonoBehaviour
    {
        private bool _canGiveInput;
        private bool _isTakingInput;
        private bool _pieceMoved;
        private Vector2 _inputStartPos;

        private ASignal<bool> _verticalSpeedSignal;
        private ASignal<float> _horizontalInputSignal;
        private ASignal _rotateSignal;

        private void Awake()
        {
            RegisterSignals();
        }

        private void RegisterSignals()
        {
            Signals.Get<GameplayStarted>().AddListener(OnGameplayStarted);
            Signals.Get<LevelFinished>().AddListener(OnLevelFinished);
            Signals.Get<PauseRequested>().AddListener(OnPaused);
            Signals.Get<PauseCanceled>().AddListener(OnPauseCanceled);
            Signals.Get<CurrentPieceChanged>().AddListener(OnCurrentPieceChanged);
            
            _verticalSpeedSignal = Signals.Get<VerticalSpeedToggled>();
            _horizontalInputSignal = Signals.Get<HorizontalInputGiven>();
            _rotateSignal = Signals.Get<RotateInputGiven>();
        }

        private void Update()
        {
            if (!_canGiveInput) return;

            HandleInputs();
        }

        private void HandleInputs()
        {
            if (InputManager.GetMouseButtonDown())
            {
                StartTakingInput();
            }

            if (!_isTakingInput) return;

            if (InputManager.GetMouseButton())
            {
                ProcessDraggingInput();
            }

            if (InputManager.GetMouseButtonUp() && !_pieceMoved)
            {
                _rotateSignal.Dispatch();
            }
        }

        private void StartTakingInput()
        {
            _inputStartPos = InputManager.GetMousePosition();
            _pieceMoved = false;
            _isTakingInput = true;
        }

        private void ProcessDraggingInput()
        {
            var currentInputPos = InputManager.GetMousePosition();
            var delta = _inputStartPos - currentInputPos;

            if (Mathf.Abs(delta.y) > 10f)
            {
                _verticalSpeedSignal.Dispatch(true);
                _pieceMoved = true;
            }
            else if (Mathf.Abs(delta.x) > 10f)
            {
                _horizontalInputSignal.Dispatch(InputManager.DeltaMousePosition.x);
                _inputStartPos = currentInputPos;
                delta.x = 0;
                _pieceMoved = true;
            }
        }

        private void OnCurrentPieceChanged(Piece piece)
        {
            _isTakingInput = false;
        }

        private void OnPauseCanceled()
        {
            _canGiveInput = true;
        }

        private void OnPaused()
        {
            _canGiveInput = false;
        }

        private void OnLevelFinished()
        {
            _canGiveInput = false;
        }

        private void OnGameplayStarted(GameMode mode)
        {
            _canGiveInput = true;
        }
    }
}
