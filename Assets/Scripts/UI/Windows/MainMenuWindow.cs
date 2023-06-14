using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class MainMenuWindow : AWindowController
    {
        [SerializeField] private Button _playButton;

        [SerializeField] private Button _continueButton;

        protected override void Awake()
        {
            base.Awake();
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        private void OnContinueButtonClicked()
        {
            Signals.Get<ContinueButtonClicked>().Dispatch();
        }
        
        private void OnPlayButtonClicked()
        {
            Signals.Get<PlayButtonClicked>().Dispatch();
        }
    }
}