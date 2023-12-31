using Enums;
using Game.Managers;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class GameplayController : MonoBehaviour
    {
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
                    var boardWidth = _playerBoardController.GetWidth();
                    var targetPos = _playerBoardController.transform.position + Vector3.right *
                        (boardWidth + ConfigHelper.Config.DistanceBetweenBoards);
                    _opponentBoardController = Instantiate(_aiControllerPrefab, targetPos, Quaternion.identity);
                }
                else
                {
                    _opponentBoardController.ResetBoard();
                }

                _opponentBoardController.Initialize();
            }
            else
            {
                if (_opponentBoardController != null)
                {
                    _opponentBoardController.gameObject.SetActive(false);
                }
            }
        }
    }
}