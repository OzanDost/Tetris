using Enums;
using Game.Managers;
using ThirdParty;
using ThirdParty.uiframework;
using UnityEngine;

namespace UI.Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("References")] public GameManager gameManager;

        [SerializeField] private UISettings defaultUISettings;

        private UIFrame _uiFrame;

        private void Awake()
        {
            _uiFrame = defaultUISettings.CreateUIInstance();

            // Subscribing to events
            Signals.Get<GameStateChanged>().AddListener(GameManager_OnGameStateChanged);
            Signals.Get<TogglePause>().AddListener(OnToggledPause);
        }

        private void OnToggledPause(bool isPaused)
        {
            if (isPaused) _uiFrame.OpenWindow("PausePopup");
        }

        private void GameManager_OnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Loading:
                    _uiFrame.OpenWindow("FakeLoadingWindow");
                    break;
                case GameState.Menu:
                    _uiFrame.CloseWindow("GameplayWindow");
                    _uiFrame.OpenWindow("MainMenuWindow");
                    break;
                case GameState.Fail:
                    _uiFrame.OpenWindow("FailPopup");
                    break;
                case GameState.Success:
                    _uiFrame.OpenWindow("SuccessPopup");
                    break;
                case GameState.Gameplay:
                    _uiFrame.OpenWindow("GameplayWindow");
                    break;
            }
        }
    }
}