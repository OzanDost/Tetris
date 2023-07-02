using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Enums;
using Game;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public class GameConfigEditor : EditorWindow
    {
        private const string GameConfigFilePath = "GameConfig";

        // [OnValueChanged("OnConfigChanged")]
        [SerializeField]
        private GameConfig _gameConfig;

        private SerializedObject _serializedGameConfig;

        [SerializeField] private PieceWeightsDictionary _pieceWeightsDictionary;

        private SerializedProperty _pieceWeightsDictionaryProp;
        private SerializedProperty _pieceMovementConfigProp;
        private SerializedProperty _winLoseConditionConfigProp;
        private SerializedProperty _cameraConfigProp;

        [MenuItem("Tools/Game Config Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameConfigEditor>("Game Config Editor");
            window.ShowPopup();
        }

        private void OnEnable()
        {
            _gameConfig = Resources.Load<GameConfig>(GameConfigFilePath);
            _serializedGameConfig = new SerializedObject(_gameConfig);

            FindProperties();
        }

        private void FindProperties()
        {
            _pieceWeightsDictionaryProp = _serializedGameConfig.FindProperty("_pieceWeightsDictionary");
            _pieceMovementConfigProp = _serializedGameConfig.FindProperty("_pieceMovementConfig");
            _winLoseConditionConfigProp = _serializedGameConfig.FindProperty("_winLoseConditionConfig");
            _cameraConfigProp = _serializedGameConfig.FindProperty("_cameraConfig");
        }

        private void OnGUI()
        {
            if (_serializedGameConfig == null)
                return;

            _serializedGameConfig.Update();

            EditorGUILayout.BeginVertical("box");
            // Piece Weights Dictionary
            EditorGUILayout.PropertyField(_pieceWeightsDictionaryProp, true);
            if (GUILayout.Button("Reset Dictionary"))
            {
                ResetDictionary();
            }

            EditorGUILayout.EndVertical();


            // Piece Movement Config
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Piece Movement Config", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_pieceMovementConfigProp.FindPropertyRelative("horizontalMoveSpeed"));
            EditorGUILayout.PropertyField(_pieceMovementConfigProp.FindPropertyRelative("_horizontalMoveStep"));
            EditorGUILayout.PropertyField(_pieceMovementConfigProp.FindPropertyRelative("_verticalMoveSpeed"));
            EditorGUILayout.PropertyField(_pieceMovementConfigProp.FindPropertyRelative("_verticalFastMoveSpeed"));
            EditorGUILayout.EndVertical();

            // Win & Lose Conditions
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Win & Lose Conditions", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_winLoseConditionConfigProp.FindPropertyRelative("_allowedMistakeCount"));
            EditorGUILayout.PropertyField(_winLoseConditionConfigProp.FindPropertyRelative("_targetHeight"));
            EditorGUILayout.EndVertical();

            // Camera Configs
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Camera Config", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(
                _cameraConfigProp.FindPropertyRelative("_additionalVerticalOffsetSingleMode"));
            EditorGUILayout.PropertyField(
                _cameraConfigProp.FindPropertyRelative("_additionalVerticalOffsetVersusMode"));
            EditorGUILayout.EndVertical();

            _serializedGameConfig.ApplyModifiedProperties();
        }

        private void OnDestroy()
        {
            AssetDatabase.SaveAssetIfDirty(_gameConfig);
        }

        private void ResetDictionary()
        {
            var existingPieces = AssetDatabase.FindAssets($"t:Prefab Piece_")
                .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(piece => piece != null)
                .Select(piece => piece.GetComponent<Piece>())
                .Where(pieceComponent => pieceComponent != null);

            _gameConfig.PieceWeightsDictionary = new PieceWeightsDictionary();

            foreach (var existingPiece in existingPieces)
            {
                _gameConfig.PieceWeightsDictionary.TryAdd(existingPiece.PieceType, 1);
            }

            EditorUtility.SetDirty(this);

            Repaint();
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

    [CustomPropertyDrawer(typeof(PieceWeightsDictionary))]
    public class CustomPieceWeightDictionaryDrawer : SerializableDictionaryPropertyDrawer
    {
    }
}