using UnityEngine;

namespace Game
{
    public class Zone : MonoBehaviour
    {
        [SerializeField] private Transform[] _horizontalBounds;

        public Transform[] HorizontalBounds => _horizontalBounds;
    }
}