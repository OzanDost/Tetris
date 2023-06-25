using System;
using UnityEngine;

namespace Game
{
    public class StageFinishLine : Zone
    {
        public event Action<Collider2D> PieceReachedStageTarget;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PieceReachedStageTarget?.Invoke(other);
        }

        public void SetLocalHeight(float height)
        {
            var position = transform.localPosition;
            position = new Vector3(position.x, height, position.z);
            transform.localPosition = position;
        }
    }
}