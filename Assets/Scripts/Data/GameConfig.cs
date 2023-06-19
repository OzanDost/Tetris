using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Data/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public PieceWeightsDictionary PieceWeightsDictionary => pieceWeightsDictionary;

        [SerializeField] private PieceWeightsDictionary pieceWeightsDictionary;

        #region Piece Configs

        public float HorizontalMoveSpeed => horizontalMoveSpeed;
        private float horizontalMoveSpeed;

        public float VerticalMoveSpeed => verticalMoveSpeed;
        private float verticalMoveSpeed;

        public float VerticalFastMoveSpeed => verticalFastMoveSpeed;
        private float verticalFastMoveSpeed;

        #endregion

        #region Gameplay Configs

        public int[] StageHeights => _stageHeights;
        private int[] _stageHeights;

        #endregion
    }
}