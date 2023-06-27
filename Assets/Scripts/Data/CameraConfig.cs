using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class CameraConfig
    {
        [BoxGroup("Camera Configs")]
        [SerializeField] private float _additionalVerticalOffsetSingleMode;

        public float AdditionalVerticalOffsetSingleMode => _additionalVerticalOffsetSingleMode;

        [BoxGroup("Camera Configs")]
        [SerializeField] private float _additionalVerticalOffsetVersusMode;

        public float AdditionalVerticalOffsetVersusMode => _additionalVerticalOffsetVersusMode;
    }
}