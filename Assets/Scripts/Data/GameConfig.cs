using System.Linq;
using Game;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public PieceWeightsDictionary PieceWeightsDictionary
        {
            get => pieceWeightsDictionary;
            set => pieceWeightsDictionary = value;
        }

        [BoxGroup("Piece Weights")]
        [SerializeField] private PieceWeightsDictionary pieceWeightsDictionary;

       
        [HideLabel]
        [SerializeField] private PieceMovementConfig _pieceMovementConfig;

        public PieceMovementConfig PieceMovementConfig => _pieceMovementConfig;

        [HideLabel]
        [SerializeField] private WinLoseConditionConfig _winLoseConditionConfig;

        public WinLoseConditionConfig WinLoseConditionConfig => _winLoseConditionConfig;

        #region Board Configs

        [SerializeField] private int _distanceBetweenBoards;

        public int DistanceBetweenBoards
        {
            get => _distanceBetweenBoards;
            set => _distanceBetweenBoards = value;
        }

        #endregion

        [HideLabel]
        [SerializeField]
        private CameraConfig _cameraConfig;

        public CameraConfig CameraConfig => _cameraConfig;


#if UNITY_EDITOR


#endif
    }
}