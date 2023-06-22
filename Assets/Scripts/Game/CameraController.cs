using System.Collections.Generic;
using Cinemachine;
using Enums;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Transform _cameraLookAtTarget;

        private Bounds _bounds;
        private List<Transform> _targets;

        private void Awake()
        {
            Signals.Get<BoardArranged>().AddListener(OnBoardArranged);
            Signals.Get<GameStateChanged>().AddListener(OnGameStateChanged);
            Signals.Get<AIMistakesFilled>().AddListener(OnAiMistakesFilled);

            _targets = new List<Transform>(4);
        }

        private void OnAiMistakesFilled()
        {
            //remove last 3 elements
            _targets.RemoveRange(_targets.Count - 3, 3);
            CreateViewBox();
        }

        private void OnGameStateChanged(GameState oldState, GameState newState)
        {
            if (newState is not GameState.Gameplay)
            {
                _bounds = new Bounds();
                _targets.Clear();
            }
        }

        private void OnBoardArranged(Transform[] groundBounds, Transform pieceSpawner)
        {
            _targets.AddRange(groundBounds);
            _targets.Add(pieceSpawner);

            CreateViewBox();
        }

        private void CreateViewBox()
        {
            foreach (Transform target in _targets)
            {
                _bounds.Encapsulate(target.position);
            }

            SetOrthographicSizeToFitBounds(_bounds, 0.75f);
            _cameraLookAtTarget.position = new Vector3(_bounds.center.x, _bounds.size.y  / 3f, _bounds.center.z);
        }

        private void SetOrthographicSizeToFitBounds(Bounds targetBounds, float paddingFactor)
        {
            float cameraAspect = _virtualCamera.m_Lens.Aspect;
            float boundsAspect = targetBounds.size.x / targetBounds.size.y;

            float orthographicSize;

            // If camera aspect is greater than the bounds aspect, calculate size based on width
            if (cameraAspect > boundsAspect)
            {
                orthographicSize = targetBounds.extents.x / cameraAspect;
            }
            else
            {
                // Otherwise, calculate size based on height
                orthographicSize = targetBounds.extents.y;
            }

            // Add padding
            orthographicSize += orthographicSize * paddingFactor;

            _virtualCamera.m_Lens.OrthographicSize = orthographicSize;
        }
    }
}