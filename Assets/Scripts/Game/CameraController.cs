using System.Collections.Generic;
using Cinemachine;
using Enums;
using Game.Managers;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Transform _cameraLookAtTarget;
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        private Bounds _bounds;
        private List<List<Transform>> _targets;


        private void Awake()
        {
            Signals.Get<BoardArranged>().AddListener(OnBoardArranged);
            Signals.Get<GameStateChanged>().AddListener(OnGameStateChanged);
            Signals.Get<CurrentPieceChanged>().AddListener(OnCurrentPieceChanged);
            _targets = new List<List<Transform>>(2);
        }

        private void OnCurrentPieceChanged(Piece piece)
        {
            ShakeCamera();
        }

        private void ShakeCamera()
        {
            _impulseSource.GenerateImpulse(0.1f);
        }

        private void OnGameStateChanged(GameState oldState, GameState newState)
        {
            if (newState is not GameState.Gameplay)
            {
                _bounds = new Bounds();
                _targets.Clear();
            }
        }

        private void OnBoardArranged(Transform[] groundBounds, Transform[] finishBounds, Transform pieceSpawner)
        {
            var boardTargets = new List<Transform>(groundBounds.Length + finishBounds.Length + 1);
            boardTargets.AddRange(groundBounds);
            boardTargets.AddRange(finishBounds);
            boardTargets.Add(pieceSpawner);

            _targets.Add(boardTargets);
            CreateViewBox();
        }

        private void CreateViewBox()
        {
            foreach (List<Transform> targetList in _targets)
            {
                foreach (Transform target in targetList)
                {
                    _bounds.Encapsulate(target.position);
                }
            }

            var targetPadding = _targets.Count > 1 ? 1.75f : 1f;
            var targetOffset = _targets.Count > 1
                ? ConfigHelper.Config.CameraConfig.AdditionalVerticalOffsetVersusMode
                : ConfigHelper.Config.CameraConfig.AdditionalVerticalOffsetSingleMode;

            SetOrthographicSizeToFitBounds(_bounds, targetPadding);
            _cameraLookAtTarget.position = _bounds.center + Vector3.up * targetOffset;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }

        private void SetOrthographicSizeToFitBounds(Bounds targetBounds, float paddingFactor)
        {
            float orthographicSize = targetBounds.size.x > targetBounds.size.y
                ? targetBounds.size.x * Screen.height / Screen.width * 0.5f
                : targetBounds.size.y * 0.5f;

            orthographicSize *= paddingFactor;

            _virtualCamera.m_Lens.OrthographicSize = orthographicSize;
        }
    }
}