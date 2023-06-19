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
        }

        private void GameManager_OnGameStateChanged(GameState oldState, GameState newState)
        {
            if (newState == GameState.Loading)
            {
                _uiFrame.OpenWindow("FakeLoadingWindow");
            }

            if (newState == GameState.Menu)
            {
                _uiFrame.CloseWindow("GameplayWindow");
                _uiFrame.OpenWindow("MainMenuWindow");
            }

            if (newState == GameState.Fail)
            {
                _uiFrame.OpenWindow("FailPopup");
            }

            if (newState == GameState.Gameplay)
            {
                _uiFrame.OpenWindow("GameplayWindow");
            }
        }
    }
}