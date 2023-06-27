using Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class GameConfigEditor : OdinEditorWindow
    {
        private const string GameConfigFilePath = "GameConfig";

        // [OnValueChanged("OnConfigChanged")]
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
    }
}