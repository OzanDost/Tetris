using ThirdParty;
using ThirdParty.uiframework.Window;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class GameplayWindow : AWindowController
    {
        [SerializeField] private Image[] _lives;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _lightningButton;
        [SerializeField] private Button _freezeButton;
        [SerializeField] private TextMeshProUGUI _manaText;
        [SerializeField] private TextMeshProUGUI _scoreText;

        private int _currentLiveIndex;

        protected override void Awake()
        {
            base.Awake();
            _pauseButton.onClick.AddListener(OnPauseButtonClicked);
            _lightningButton.onClick.AddListener(OnLightningButtonClicked);
            _freezeButton.onClick.AddListener(OnFreezeButtonClicked);

            Signals.Get<LifeLost>().AddListener(OnLifeLost);
        }

        protected override void On_UIOPen()
        {
            base.On_UIOPen();
            _currentLiveIndex = 0;
            //todo get this from save data
        }

        private void OnLifeLost()
        {
            //todo close one of the hearts and show a cross mark at the bottom of the screen
            if (_currentLiveIndex >= _lives.Length)
                return;

            _currentLiveIndex++;
            
            for (int i = 0; i < _currentLiveIndex; i++)
            {
                _lives[i].gameObject.SetActive(false);
            }
        }

        private void OnLightningButtonClicked()
        {
            //todo 
        }

        private void OnFreezeButtonClicked()
        {
        }

        private void OnPauseButtonClicked()
        {
            Signals.Get<PauseRequested>().Dispatch();
        }
    }
}