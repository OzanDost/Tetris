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
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        private Bounds _bounds;
        private List<List<Transform>> _targets;


        private void Awake()
        {
            Signals.Get<BoardArranged>().AddListener(OnBoardArranged);
            Signals.Get<GameStateChanged>().AddListener(OnGameStateChanged);
            Signals.Get<AIMistakesFilled>().AddListener(OnAiMistakesFilled);
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

            SetOrthographicSizeToFitBounds(_bounds, 1.25f);
            // _cameraLookAtTarget.position = new Vector3(_bounds.center.x, _bounds.size.y / 3f, _bounds.center.z);
            _cameraLookAtTarget.position = _bounds.center;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }

        private void SetOrthographicSizeToFitBounds(Bounds targetBounds, float paddingFactor)
        {
            float orthographicSize;

            orthographicSize = targetBounds.size.x;

            // Add padding
            orthographicSize *= paddingFactor;

            _virtualCamera.m_Lens.OrthographicSize = orthographicSize;
        }
    }
}