using DG.Tweening;
using Game;
using ThirdParty;
using UnityEngine;

namespace DefaultNamespace
{
    public class PieceColorizer : MonoBehaviour
    {
        private void Awake()
        {
            Signals.Get<PiecePlaced>().AddListener(HighlightPiece);
        }

        private void HighlightPiece(Piece piece)
        {
            var highlightSequence = DOTween.Sequence();
            var pieceColor = piece.SpriteRenderers[0].color;

            // Set target color to be a bit whiter than pieceColor
            float whitenessFactor = 0.3f;
            var targetColor = new Color(
                Mathf.Clamp01(pieceColor.r + whitenessFactor),
                Mathf.Clamp01(pieceColor.g + whitenessFactor),
                Mathf.Clamp01(pieceColor.b + whitenessFactor),
                pieceColor.a // Keep the same alpha value
            );

            foreach (var spriteRenderer in piece.SpriteRenderers)
            {
                highlightSequence.Join(spriteRenderer.DOColor(targetColor, 0.1f).SetLoops(2, LoopType.Yoyo));
            }
        }
    }
}