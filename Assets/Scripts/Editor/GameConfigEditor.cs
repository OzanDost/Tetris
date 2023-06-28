using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Game;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public class GameConfigEditor : OdinEditorWindow
    {
        private const string GameConfigFilePath = "GameConfig";

        // [OnValueChanged("OnConfigChanged")]
        [SerializeField]
        private GameConfig _gameConfig;

        [HideLabel]
        [OnValueChanged("OnConfigChanged")]
        [SerializeField] private PieceWeightsDictionary _pieceWeightsDictionary;

        [HideLabel]
        [OnValueChanged("OnConfigChanged")]
        [SerializeField] private PieceMovementConfig _pieceMovementConfig;

        [HideLabel]
        [OnValueChanged("OnConfigChanged")]
        [SerializeField] private WinLoseConditionConfig _winLoseConditionConfig;

        [HideLabel]
        [OnValueChanged("OnConfigChanged")]
        [SerializeField] private CameraConfig _cameraConfig;

        [OnValueChanged("OnConfigChanged")]
        [SerializeField] private int _distanceBetweenBoards;

        public void Init()
        {
            _gameConfig = Resources.Load<GameConfig>(GameConfigFilePath);

            _pieceWeightsDictionary = _gameConfig.PieceWeightsDictionary;
            _pieceMovementConfig = _gameConfig.PieceMovementConfig;
            _winLoseConditionConfig = _gameConfig.WinLoseConditionConfig;
            _cameraConfig = _gameConfig.CameraConfig;
            _distanceBetweenBoards = _gameConfig.DistanceBetweenBoards;


            OnClose -= OnClosed;
            OnClose += OnClosed;
        }


        private void OnClosed()
        {
            AssetDatabase.SaveAssetIfDirty(_gameConfig);
        }

        private void OnConfigChanged()
        {
            _gameConfig.DistanceBetweenBoards = _distanceBetweenBoards;
            EditorUtility.SetDirty(_gameConfig);
        }
        [Button, BoxGroup("Piece Weights")]
        private void ResetDictionary()
        {
            
            var existingPieces = AssetDatabase.FindAssets($"t:Prefab Piece_")
                .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(piece => piece != null)
                .Select(piece => piece.GetComponent<Piece>())
                .Where(pieceComponent => pieceComponent != null);


            _pieceWeightsDictionary = new PieceWeightsDictionary();

            foreach (var existingPiece in existingPieces)
            {
                _pieceWeightsDictionary.TryAdd(existingPiece.PieceType, 1);
            }

            EditorUtility.SetDirty(this);
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

                foreach (var pieceType in gameConfig.PieceWeightsDictionary.Keys)
                {
                    if (!Enum.IsDefined(typeof(PieceType), pieceType))
                    {
                        _pieceTypesToRemove.Add(pieceType);
                    }
                }

                foreach (var pieceType in _pieceTypesToRemove)
                {
                    gameConfig.PieceWeightsDictionary.Remove(pieceType);
                }

                EditorUtility.SetDirty(gameConfig);
            }
        }
    }
}