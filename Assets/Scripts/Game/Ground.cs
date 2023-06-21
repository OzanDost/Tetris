using UnityEngine;

namespace Game
{
    public class Ground : MonoBehaviour
    {
        [SerializeField] private Transform[] _horizontalBounds;

        public Transform[] HorizontalBounds => _horizontalBounds;
    }
}