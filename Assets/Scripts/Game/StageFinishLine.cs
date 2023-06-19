using System;
using UnityEngine;

namespace Game
{
    public class StageFinishLine : MonoBehaviour
    {
        public event Action<Collider2D> PieceReachedStageTarget;

        private void OnTriggerEnter2D(Collider2D other)
        {
            gameObject.SetActive(false);
            PieceReachedStageTarget?.Invoke(other);
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    }
}