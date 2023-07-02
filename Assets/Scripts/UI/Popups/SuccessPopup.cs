using DG.Tweening;
using ThirdParty;
using ThirdParty.uiframework.Window;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    public class SuccessPopup : AWindowController
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [SerializeField] private RectTransform[] stars;

        private Tween _scoreTween;
        private Sequence _starSequence;
        private float _targetScore;


        protected override void AddListeners()
        {
            base.AddListeners();
            continueButton.onClick.AddListener(OnContinueButtonClicked);

            Signals.Get<BoardHeightCalculated>().AddListener(OnBoardHeightCalculated);
        }

        private void OnBoardHeightCalculated(float height)
        {
            _targetScore = height;
        }

        private void AnimateScoreText()
        {
            _scoreTween.Kill();
            _scoreTween = DOTween.To(() => 0, x => scoreText.SetText($"Score: {x:F1}m"), _targetScore, 2f)
                .SetEase(Ease.InOutExpo);
        }

        protected override void On_UIOPen()
        {
            base.On_UIOPen();

            foreach (var star in stars)
            {
                star.gameObject.SetActive(false);
            }

            _starSequence = DOTween.Sequence();
            foreach (var star in stars)
            {
                _starSequence.Append(star.DOScale(1, 0.4f).From(2.3f).SetEase(Ease.InExpo)
                    .OnStart(() => star.gameObject.SetActive(true)));
            }

            float startNumber = 0;

            _scoreTween = DOTween.To(() => startNumber, x => startNumber = x, _targetScore, 2f)
                .SetEase(Ease.InOutExpo)
                .OnUpdate(() => { scoreText.SetText($"Score: {startNumber}"); });

            AnimateScoreText();
        }

        protected override void On_UIClose()
        {
            base.On_UIClose();

            _starSequence?.Kill(true);
            _scoreTween?.Kill(true);
        }

        private void OnContinueButtonClicked()
        {
            Signals.Get<LevelQuit>().Dispatch();
        }
    }
}