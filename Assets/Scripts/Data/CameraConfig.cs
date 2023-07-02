using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class CameraConfig
    {
        [SerializeField] private float _additionalVerticalOffsetSingleMode;

        public float AdditionalVerticalOffsetSingleMode => _additionalVerticalOffsetSingleMode;

        [SerializeField] private float _additionalVerticalOffsetVersusMode;

        public float AdditionalVerticalOffsetVersusMode => _additionalVerticalOffsetVersusMode;
    }
}