using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class PausePopup : AWindowController
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _backgroundTapButton;
        [SerializeField] private Button _quitButton;

        protected override void Awake()
        {
            base.Awake();
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            _backgroundTapButton.onClick.AddListener(OnResumeButtonClicked);
            _quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnQuitButtonClicked()
        {
            Signals.Get<LevelQuit>().Dispatch();
            Signals.Get<TogglePause>().Dispatch(false);
        }

        private void OnResumeButtonClicked()
        {
            CloseRequest?.Invoke(this);
            Signals.Get<TogglePause>().Dispatch(false);
        }
    }
}