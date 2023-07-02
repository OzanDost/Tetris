using Data;
using Game;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    public class PoolConfigEditor : EditorWindow
    {
        private const string PoolConfigFilePath = "PoolConfig";

        private PoolConfig _poolConfig;
        private SerializedObject _serializedObject;
        private ReorderableList _reorderableList;

        [MenuItem("Tools/Pool Config Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<PoolConfigEditor>();
            window.Show();
        }

        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            _poolConfig = Resources.Load<PoolConfig>(PoolConfigFilePath);
            _serializedObject = new SerializedObject(_poolConfig);

            _reorderableList = new ReorderableList(_serializedObject, _serializedObject.FindProperty("items"), true,
                true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Poolable Items"),
                drawElementCallback = DrawItem,
                elementHeight = 100f,
            };
        }



        private void DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var generationCount = element.FindPropertyRelative("InitialGenerationCount");
            rect.y += 2;

            Rect titleRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            float fieldWidth = rect.width * 0.75f;

            Rect pieceRect = new Rect(rect.x, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            Rect countRect = new Rect(rect.x, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            Rect previewRect = new Rect(rect.x + fieldWidth + 50, rect.y - EditorGUIUtility.singleLineHeight * 2 - 4,
                64, 64);

            EditorGUI.LabelField(titleRect, _poolConfig.Items[index].Piece.name, EditorStyles.boldLabel);

            Piece piece = (Piece)EditorGUI.ObjectField(pieceRect,
                new GUIContent("Piece", "Reference to the piece object"),
                element.FindPropertyRelative("Piece").objectReferenceValue,
                typeof(Piece),
                false);

            var newGenerationCount = EditorGUI.IntField(countRect,
                new GUIContent("Initial Generation Count", "Number of pieces that will be generated initially"),
                generationCount.intValue);


            if (piece != null)
            {
                Texture2D previewImage = AssetPreview.GetAssetPreview(piece.gameObject);
                if (previewImage != null)
                    EditorGUI.DrawPreviewTexture(previewRect, previewImage);
            }

            generationCount.intValue = newGenerationCount;
        }


        private void OnGUI()
        {
            if (_poolConfig == null)
            {
                GUILayout.Label("No Pool Config or Items found.");
                return;
            }

            if (GUILayout.Button("Init"))
            {
                Init();
            }

            if (_serializedObject == null) return;

            _reorderableList.DoLayoutList();

            if (GUI.changed)
            {
                _serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_poolConfig);
                AssetDatabase.SaveAssetIfDirty(_poolConfig);
            }
        }


        private void OnDestroy()
        {
            AssetDatabase.SaveAssetIfDirty(_poolConfig);
        }
    }
    
    
}