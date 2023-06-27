using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Game;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Callbacks;
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
            var existingPieces = Utils.GetSavedPieces();

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


#if UNITY_EDITOR

        [Button]
        public void Set()
        {
            EditorPrefHelper.SetLastAmountOfPieceTypes(pieceWeightsDictionary.Count);
        }

        [DidReloadScripts]
        private static void CheckPieceTypesChanged()
        {
            var lastAmountOfPieces = EditorPrefHelper.GetLastAmountOfPieceTypes();
            var currentPieceTypes = Enum.GetValues(typeof(PieceType));
            if (currentPieceTypes.Length < lastAmountOfPieces)
            {
                EditorPrefHelper.SetLastAmountOfPieceTypes(Enum.GetValues(typeof(PieceType)).Length);

                var gameConfig = Resources.Load<GameConfig>("GameConfig");

                List<PieceType> _pieceTypesToRemove = new List<PieceType>();

                foreach (var pieceType in gameConfig.pieceWeightsDictionary.Keys)
                {
                    if (!Enum.IsDefined(typeof(PieceType), pieceType))
                    {
                        _pieceTypesToRemove.Add(pieceType);
                    }
                }

                foreach (var pieceType in _pieceTypesToRemove)
                {
                    gameConfig.pieceWeightsDictionary.Remove(pieceType);
                }

                EditorUtility.SetDirty(gameConfig);
            }
        }
#endif
    }
}