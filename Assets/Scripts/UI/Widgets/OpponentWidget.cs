using DG.Tweening;
using Enums;
using ThirdParty;
using TMPro;
using UnityEngine;

namespace UI.Widgets
{
    public class OpponentWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _height;

        private float _showX;
        private float _hideX;
        private Tween _moveTween;

        private void Awake()
        {
            Signals.Get<AIBoardArranged>().AddListener(OnAiBoardArranged);
            Signals.Get<AIBoardHeightChanged>().AddListener(OnAiBoardHeightChanged);
            Signals.Get<AIMistakesFilled>().AddListener(OnAiMistakesFilled);
            Signals.Get<GameplayStarted>().AddListener(OnGameplayStarted);
            _showX = transform.localPosition.x;
            _hideX = _showX + 200f;
        }

        private void OnGameplayStarted(GameMode mode)
        {
            gameObject.SetActive(mode == GameMode.Versus);
        }

        private void OnAiMistakesFilled()
        {
            Hide();
        }

        private void Hide()
        {
            _height.SetText("Failed");
            
            _moveTween?.Kill();
            _moveTween = transform.DOLocalMoveX(_hideX, 0.5f).SetEase(Ease.InOutExpo).SetDelay(0.5f);
        }

        private void OnAiBoardHeightChanged(float boardHeight)
        {
            SetHeight(boardHeight);
            Show();
        }

        private void Show()
        {
            _moveTween?.Kill();
            _moveTween = transform.DOLocalMoveX(_showX, 0.5f).SetEase(Ease.InOutExpo);
        }

        private void OnAiBoardArranged(Transform[] arg1, Transform[] arg2, Transform arg3)
        {
            SetHeight(0f);
        }

        public void SetHeight(float towerHeight)
        {
            _height.text = $"{towerHeight:F1}m";
        }
    }
}