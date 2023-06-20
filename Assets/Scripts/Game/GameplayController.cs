using System;
using Enums;
using Game.Managers;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class GameplayController : MonoBehaviour
    {
        private bool _canGiveInput;
        private GameMode _gameMode;

        private BoardController _playerBoardController;
        private BoardController _opponentBoardController;

        private Vector2 _inputStartPos;
        private bool _pieceMoved;

        [SerializeField] private BoardController _boardControllerPrefab;

        private void Awake()
        {
            Signals.Get<GameplayStarted>().AddListener(OnGameplayStarted);
        }

        private void OnGameplayStarted(GameMode mode)
        {
            Initialize(mode);
            _canGiveInput = true;
        }

        private void Initialize(GameMode gameMode)
        {
            _gameMode = gameMode;

            //todo add cases for versus mode
            CreateBoardControllers(gameMode);
        }

        private void CreateBoardControllers(GameMode mode)
        {
            //todo add cases for versus mode

            if (_playerBoardController == null)
            {
                _playerBoardController = Instantiate(_boardControllerPrefab);
            }
            else
            {
                _playerBoardController.ResetBoard();
            }
        }

        private void Update()
        {
            if (!_canGiveInput) return;


            if (InputManager.GetMouseButtonDown())
            {
                _inputStartPos = InputManager.GetMousePosition();
                _pieceMoved = false;
            }

            if (InputManager.GetMouseButton())
            {
                var currentInputPos = InputManager.GetMousePosition();
                var delta = _inputStartPos - currentInputPos;

                if (Mathf.Abs(delta.y) > 10f)
                {
                    _playerBoardController.ToggleVerticalSpeed(true);
                    _pieceMoved = true;
                    return;
                }

                if (Mathf.Abs(delta.x) > 3f)
                {
                    _playerBoardController.MovePieceHorizontally(InputManager.DeltaMousePosition);
                    _inputStartPos = currentInputPos;
                    _pieceMoved = true;
                    return;
                }
            }

            if (InputManager.GetMouseButtonUp() && !_pieceMoved)
            {
                _playerBoardController.RotatePiece();
            }
        }
    }
}