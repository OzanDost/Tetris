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

        public void SetLocalHeight(int height)
        {
            var position = transform.localPosition;
            position = new Vector3(position.x, height, position.z);
            transform.localPosition = position;
            gameObject.SetActive(true);
        }

        public void IncreaseHeight(int additionalHeight)
        {
            var position = transform.position;
            position = new Vector3(position.x, position.y + additionalHeight, position.z);
            transform.position = position;
            gameObject.SetActive(true);
        }
    }
}