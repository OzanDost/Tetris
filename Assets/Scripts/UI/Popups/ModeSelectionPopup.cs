using Enums;
using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class ModeSelectionPopup : AWindowController
    {
        [SerializeField] private Button _soloModeButton;
        [SerializeField] private Button _multiplayerModeButton;
        [SerializeField] private Button _closeButton;

        protected override void Awake()
        {
            base.Awake();
            _soloModeButton.onClick.AddListener(OnSoloModeButtonClicked);
            _multiplayerModeButton.onClick.AddListener(OnMultiplayerModeButtonClicked);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnCloseButtonClicked()
        {
            CloseRequest?.Invoke(this);
        }

        private void OnMultiplayerModeButtonClicked()
        {
            Signals.Get<GameplayRequested>().Dispatch(GameMode.Versus);
        }

        private void OnSoloModeButtonClicked()
        {
            Signals.Get<GameplayRequested>().Dispatch(GameMode.Solo);
        }
    }
}