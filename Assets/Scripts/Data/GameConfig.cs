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
        [SerializeField] private float horizontalMoveSpeed;

        public float VerticalMoveSpeed => verticalMoveSpeed;
        [SerializeField] private float verticalMoveSpeed;

        public float VerticalFastMoveSpeed => verticalFastMoveSpeed;
        [SerializeField] private float verticalFastMoveSpeed;

        #endregion

        #region Gameplay Configs

        public int AllowedMistakeCount => _allowedMistakeCount;
        [SerializeField] private int _allowedMistakeCount;
        
        public int TargetHeight => _targetHeight;
        [SerializeField] private int _targetHeight;

        #endregion
    }
}