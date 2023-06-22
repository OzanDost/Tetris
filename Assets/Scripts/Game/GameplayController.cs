using Enums;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class GameplayController : MonoBehaviour
    {
        private GameMode _gameMode;

        private BoardController _playerBoardController;
        private AIController _opponentBoardController;

        [SerializeField] private BoardController _boardControllerPrefab;
        [SerializeField] private AIController _aiControllerPrefab;

        private void Awake()
        {
            Signals.Get<GameplayStarted>().AddListener(OnGameplayStarted);
        }


        private void OnGameplayStarted(GameMode mode)
        {
            Initialize(mode);
        }

        private void Initialize(GameMode gameMode)
        {
            _gameMode = gameMode;

            //todo add cases for versus mode
            CreateBoardControllers(gameMode);
        }

        private void CreateBoardControllers(GameMode mode)
        {
            if (_playerBoardController == null)
            {
                _playerBoardController = Instantiate(_boardControllerPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                _playerBoardController.ResetBoard();
            }

            _playerBoardController.Initialize();

            if (mode == GameMode.Versus)
            {
                if (_opponentBoardController == null)
                {
                    //todo change here
                    var targetPos = new Vector3(20f, 0, 0);
                    _opponentBoardController = Instantiate(_aiControllerPrefab, targetPos, Quaternion.identity);
                }
                else
                {
                    _opponentBoardController.ResetBoard();
                }

                _opponentBoardController.Initialize();
            }
        }
    }
}