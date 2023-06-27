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
        public PieceWeightsDictionary PieceWeightsDictionary => pieceWeightsDictionary;

        [BoxGroup("Piece Weights")]
        [SerializeField] private PieceWeightsDictionary pieceWeightsDictionary;

        [Button, BoxGroup("Piece Weights")]
        private void ResetDictionary()
        {
            var existingPieces = AssetDatabase.FindAssets($"t:Prefab Piece_")
                .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(piece => piece != null)
                .Select(piece => piece.GetComponent<Piece>())
                .Where(pieceComponent => pieceComponent != null);

            pieceWeightsDictionary = new PieceWeightsDictionary();

            foreach (var existingPiece in existingPieces)
            {
                pieceWeightsDictionary.TryAdd(existingPiece.PieceType, 1);
            }

            EditorUtility.SetDirty(this);
        }

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
    }
}