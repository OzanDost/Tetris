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

            if (InputManager.GetMouseButtonUp())
            {
                if (!_pieceMoved)
                {
                    _rotateSignal.Dispatch();
                }

                _verticalSpeedSignal.Dispatch(false);
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
            var delta = currentInputPos - _inputStartPos;

            if (Mathf.Abs(delta.y) > 10f)
            {
                _verticalSpeedSignal.Dispatch(true);
                _pieceMoved = true;
            }
            else if (Mathf.Abs(delta.x) > 15f)
            {
                _horizontalInputSignal.Dispatch(delta.normalized.x);
                _inputStartPos = currentInputPos;
                delta.x = 0;
                _pieceMoved = true;
            }
        }

        private void OnCurrentPieceChanged(Piece piece)
        {
            _isTakingInput = false;
            _pieceMoved = false;
            _inputStartPos = InputManager.GetMousePosition();
            _verticalSpeedSignal.Dispatch(false);
        }

        private void OnPauseCanceled()
        {
            _canGiveInput = true;
        }

        private void OnPaused()
        {
            _canGiveInput = false;
        }

        private void OnLevelFinished(bool isSuccess)
        {
            _canGiveInput = false;
        }

        private void OnGameplayStarted(GameMode mode)
        {
            _canGiveInput = true;
        }
    }
}