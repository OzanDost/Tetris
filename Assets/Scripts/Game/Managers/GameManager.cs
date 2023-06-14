using DG.Tweening;
using ThirdParty;
using UnityEngine;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        public GameState CurrentGameState { get; private set; }

        private void Awake()
        {
            ApplyConfigs();
        }

        private void Start()
        {
            // Subscribing to events
            Signals.Get<RequestGameStateChange>().AddListener(OnGameStateChangeRequested);
            Signals.Get<FakeLoadingFinished>().AddListener(OnFakeLoadingFinished);
            Signals.Get<SoloModeButtonClicked>().AddListener(OnSoloModeButtonClicked);
            Signals.Get<MultiplayerModeButtonClicked>().AddListener(OnMultiplayerModeButtonClicked);

            ChangeGameState(GameState.Loading);


            ScoreManager.Initialize();
            SaveManager.Initialize();
        }

        private void OnMultiplayerModeButtonClicked()
        {
            
        }

        private void OnSoloModeButtonClicked()
        {
        }

        private void ApplyConfigs()
        {
            int refreshRate = Screen.currentResolution.refreshRate;
            Application.targetFrameRate = refreshRate % 60 == 0 ? 60 : Screen.currentResolution.refreshRate;
            DOTween.Init().SetCapacity(200, 150);
        }

        private void MainMenu_OnContinueButtonClicked()
        {
            ChangeGameState(GameState.Gameplay);
        }

        private void Success_OnContinueButtonClicked()
        {
            ChangeGameState(GameState.Menu);
        }

        private void OnReturnToMenuRequested()
        {
        }

        private void OnLevelQuit()
        {
            ChangeGameState(GameState.Menu);
        }

        private void OnLevelFailed()
        {
            ChangeGameState(GameState.Fail);
        }

        private void OnLevelRetryRequested()
        {
            ChangeGameState(GameState.Gameplay);
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
                BoardManager.SendLevelSaveRequest();
                SaveManager.SaveHintCount();
                SaveManager.SavePlayerStatsData();
            }
#endif
        }
    }

    public enum GameState
    {
        None,
        Loading,
        Menu,
        Gameplay,
        Success,
        Fail,
    }
}