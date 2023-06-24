using Enums;
using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class MainMenuWindow : AWindowController
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _versusButton;

        protected override void Awake()
        {
            base.Awake();
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _versusButton.onClick.AddListener(OnVersusButtonClicked);
        }

        private void OnVersusButtonClicked()
        {
            Signals.Get<GameplayRequested>().Dispatch(GameMode.Versus);
        }

        private void OnPlayButtonClicked()
        {
            Signals.Get<GameplayRequested>().Dispatch(GameMode.Solo);
        }
    }
}