using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class WinLoseConditionConfig
    {
        public int AllowedMistakeCount => _allowedMistakeCount;

        [SerializeField] private int _allowedMistakeCount;

        public int TargetHeight => _targetHeight;

        [SerializeField] private int _targetHeight;
    }
}