using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public PieceWeightsDictionary PieceWeightsDictionary
        {
            get => _pieceWeightsDictionary;
            set => _pieceWeightsDictionary = value;
        }

        [SerializeField] private PieceWeightsDictionary _pieceWeightsDictionary;

       
        [SerializeField] private PieceMovementConfig _pieceMovementConfig;

        public PieceMovementConfig PieceMovementConfig => _pieceMovementConfig;

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

        [SerializeField]
        private CameraConfig _cameraConfig;

        public CameraConfig CameraConfig => _cameraConfig;
    }
    
}