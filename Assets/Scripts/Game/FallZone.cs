using System;
using UnityEngine;

namespace Game
{
    public class FallZone : MonoBehaviour
    {
        public event Action<Collider2D> PieceFellOffBoard; 
        private void OnTriggerEnter2D(Collider2D other)
        {
            PieceFellOffBoard?.Invoke(other);
        }
    }
}