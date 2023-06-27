using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class WinLoseConditionConfig
    {
        public int AllowedMistakeCount => _allowedMistakeCount;

        [BoxGroup("Win & Lose Conditions")]
        [SerializeField] private int _allowedMistakeCount;

        public int TargetHeight => _targetHeight;

        [BoxGroup("Win & Lose Conditions")]
        [SerializeField] private int _targetHeight;
    }
}