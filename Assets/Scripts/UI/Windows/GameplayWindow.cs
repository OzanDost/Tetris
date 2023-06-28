using ThirdParty;
using ThirdParty.uiframework.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameplayWindow : AWindowController
    {
        [SerializeField] private Image[] _lives;
        [SerializeField] private Button _pauseButton;

        private int _currentLiveIndex;

        protected override void Awake()
        {
            base.Awake();
            _pauseButton.onClick.AddListener(OnPauseButtonClicked);

            Signals.Get<LifeLost>().AddListener(OnLifeLost);
        }
        
        protected override void On_UIOPen()
        {
            base.On_UIOPen();
            _currentLiveIndex = 0;
        }

        private void OnLifeLost()
        {
            if (_currentLiveIndex >= _lives.Length)
                return;

            _currentLiveIndex++;
            
            for (int i = 0; i < _currentLiveIndex; i++)
            {
                _lives[i].gameObject.SetActive(false);
            }
        }
        
        private void OnPauseButtonClicked()
        {
            Signals.Get<TogglePause>().Dispatch(true);
        }
    }
}