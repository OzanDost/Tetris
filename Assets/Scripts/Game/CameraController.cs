using System;
using Cinemachine;
using Enums;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private CinemachineTargetGroup _targetGroup;

        private void Awake()
        {
            Signals.Get<BoardArranged>().AddListener(OnBoardArranged);
            Signals.Get<GameStateChanged>().AddListener(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameState oldState, GameState newState)
        {
            if (newState is GameState.Fail or GameState.Success)
            {
                _targetGroup.m_Targets = new CinemachineTargetGroup.Target[] { };
            }
        }

        private void OnBoardArranged(Transform ground, Transform pieceSpawner)
        {
            _targetGroup.AddMember(ground, 2f, 1f);
            _targetGroup.AddMember(pieceSpawner, 1f, 1f);
            _virtualCamera.enabled = true;
        }
    }
}