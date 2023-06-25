using DG.Tweening;
using Enums;
using ThirdParty;
using UnityEngine;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        public GameState CurrentGameState { get; private set; }

        private GameMode _lastGameMode;


        private void Awake()
        {
            ApplyConfigs();
        }

        private void Start()
        {
            // Subscribing to events
            Signals.Get<RequestGameStateChange>().AddListener(OnGameStateChangeRequested);
            Signals.Get<FakeLoadingFinished>().AddListener(OnFakeLoadingFinished);
            Signals.Get<GameplayRequested>().AddListener(OnGameplayRequested);
            Signals.Get<LevelFinished>().AddListener(OnLevelFinished);
            Signals.Get<LevelQuit>().AddListener(OnLevelQuit);
            Signals.Get<RetryButtonClicked>().AddListener(OnRetryButtonClicked);

            //todo change here

            ChangeGameState(GameState.Loading);


            ConfigHelper.Initialize();
            ScoreManager.Initialize();
            SaveManager.Initialize();
        }

        private void OnRetryButtonClicked()
        {
            OnLevelQuit();
            OnGameplayRequested(_lastGameMode);
        }


        private void OnGameplayRequested(GameMode gameMode)
        {
            ChangeGameState(GameState.Gameplay);
            Signals.Get<GameplayStarted>().Dispatch(gameMode);
            Signals.Get<TogglePause>().Dispatch(false);
        }

        private void ApplyConfigs()
        {
            int refreshRate = Screen.currentResolution.refreshRate;
            Application.targetFrameRate = refreshRate % 60 == 0 ? 60 : Screen.currentResolution.refreshRate;
            DOTween.Init().SetCapacity(200, 150);
        }

        private void OnLevelQuit()
        {
            ChangeGameState(GameState.Menu);
        }

        private void OnLevelFinished(bool isSuccess)
        {
            if (isSuccess)
            {
                ChangeGameState(GameState.Success);
            }
            else
            {
                ChangeGameState(GameState.Fail);
            }
        }


        private void OnFakeLoadingFinished()
        {
            ChangeGameState(GameState.Menu);
        }


        private void OnGameStateChangeRequested(GameState newState)
        {
            ChangeGameState(newState);
        }

        private void ChangeGameState(GameState newGameState)
        {
            var oldGameState = CurrentGameState;
            CurrentGameState = newGameState;
            Signals.Get<GameStateChanged>().Dispatch(oldGameState, newGameState);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
#if UNITY_ANDROID || UNITY_IOS
            if (pauseStatus)
            {
            }
#endif
        }
    }
}