using Game;
using ThirdParty;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets
{
    public class NextPieceWidget : MonoBehaviour
    {
        [SerializeField] private RectTransform _confiner;
        [SerializeField] private Image[] _squares;
        [SerializeField] private float _paddingBetweenSquares;
        [SerializeField] private float _paddingFactor;

        private float _minSize;
        private int _gridSize;

        private void Awake()
        {
            Signals.Get<NextPieceChanged>().AddListener(OnNextPieceChanged);

            _minSize = Mathf.Min(_confiner.rect.width, _confiner.rect.height);
            _gridSize = 4;
        }

        
        private void OnNextPieceChanged(Piece piece)
        {
            foreach (var square in _squares)
            {
                square.gameObject.SetActive(false);
            }

            var pieceColliders = piece.Colliders;
            var color = piece.SpriteRenderers[0].color;

            float availableAreaSize = _minSize * (1 - _paddingFactor) - (_gridSize - 1) * _paddingBetweenSquares;
            float newScaleFactor = availableAreaSize / _gridSize;

            for (int i = 0; i < pieceColliders.Length; i++)
            {
                _squares[i].color = color;
                Vector3 localPositionWithPadding = new Vector3(
                    pieceColliders[i].transform.localPosition.x * (newScaleFactor + _paddingBetweenSquares),
                    pieceColliders[i].transform.localPosition.y * (newScaleFactor + _paddingBetweenSquares),
                    pieceColliders[i].transform.localPosition.z
                );
                _squares[i].rectTransform.anchoredPosition = localPositionWithPadding;
                _squares[i].rectTransform.sizeDelta =
                    new Vector2(newScaleFactor, newScaleFactor);
                _squares[i].gameObject.SetActive(true);
            }
        }
    }
}