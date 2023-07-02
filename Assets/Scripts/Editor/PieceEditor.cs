using Enums;
using Game;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PieceEditor : EditorWindow
    {
        private const string SearchPattern = "p: t:prefab Assets/Prefabs/Game/Pieces -Piece_Base";
        
        private const int MinCellSize = 128;
        private const int MaxCellSize = 200;

        [SerializeField]
        private GameObject _pieceToEdit;

        private Piece _piece;
        private PieceType _pieceType;
        private string _pieceName;
        private Sprite _pieceCellSprite;
        private Color _pieceCellColor;
        private readonly bool[,] _cellMatrix = new bool[4, 4];

        public void Init()
        {
        }

        [MenuItem("Tools/Piece Editor")]
        public static void OpenWindow()
        {
            PieceEditor pieceEditor = GetWindow<PieceEditor>();
            pieceEditor.minSize = new Vector2(800, 1200);
            pieceEditor.maxSize = new Vector2(800, 1200);
            pieceEditor.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            DrawSelectedPiece();

            DrawPieceFields();

            DrawCells();

            if (GUILayout.Button("Update Piece"))
            {
                UpdatePiece();
            }

            EditorGUILayout.EndVertical();
        }

        private void UpdatePiece()
        {
            _pieceToEdit.name = _pieceName;

            foreach (var spriteRenderer in _piece.SpriteRenderers)
            {
                spriteRenderer.sprite = _pieceCellSprite;
                spriteRenderer.color = _pieceCellColor;
            }

            var path = AssetDatabase.GetAssetPath(_pieceToEdit);
            AssetDatabase.RenameAsset(path, _pieceName);

            EditorUtility.SetDirty(_pieceToEdit);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void DrawSelectedPiece()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Select Piece", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            using (new EditorGUI.DisabledScope(true))
            {
                _pieceToEdit = (GameObject)EditorGUILayout.ObjectField(_pieceToEdit, typeof(GameObject), false);
            }

            if (GUILayout.Button("Select Piece"))
            {
                int controlID = GUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<GameObject>(_pieceToEdit, false, SearchPattern, controlID);
            }

            string commandName = Event.current.commandName;

            if (commandName == "ObjectSelectorClosed")
            {
                _pieceToEdit = (GameObject)EditorGUIUtility.GetObjectPickerObject();
                OnPieceChanged();
            }


            EditorGUILayout.EndHorizontal();
            if (_pieceToEdit == null)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            var editor = UnityEditor.Editor.CreateEditor(_pieceToEdit);
            editor.DrawPreview(GUILayoutUtility.GetRect(256, 256));
            EditorGUILayout.EndVertical();
        }

        private void DrawPieceFields()
        {
            GUILayout.BeginHorizontal(new GUIStyle()
            {
                border = new RectOffset(10, 10, 10, 10),
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(10, 10, 10, 10),
                normal =
                {
                    background = Texture2D.grayTexture,
                }
            });

            GUILayout.BeginVertical();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Piece Fields", EditorStyles.boldLabel);
            _pieceName = EditorGUILayout.TextField("Piece Name", _pieceName);
            _pieceType = (PieceType)EditorGUILayout.EnumPopup("Piece Type", _pieceType);
            _pieceCellColor = EditorGUILayout.ColorField("Piece Color", _pieceCellColor);

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            _pieceCellSprite = (Sprite)EditorGUILayout.ObjectField(_pieceCellSprite, typeof(Sprite), false,
                GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MaxHeight(85),
                GUILayout.MaxWidth(85));
            GUILayout.Label("Piece Cell Sprite");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void OnPieceChanged()
        {
            if (_pieceToEdit == null) return;
            LoadPiece();
        }

        private void LoadPiece()
        {
            _piece = _pieceToEdit.GetComponent<Piece>();

            if (_piece == null)
            {
                return;
            }

            _pieceType = _piece.PieceType;
            _pieceName = _piece.name;
            // _pieceMaterial = _piece.SpriteRenderers[0].sharedMaterial;
            _pieceCellColor = _piece.SpriteRenderers[0].color;
            _pieceCellSprite = _piece.SpriteRenderers[0].sprite;


            for (int i = 0; i < _cellMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _cellMatrix.GetLength(1); j++)
                {
                    _cellMatrix[i, j] = false;
                }
            }


            float minY = float.MaxValue;
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            float maxX = float.MinValue;

            // Calculate Min and Max positions
            foreach (var pieceCollider in _piece.Colliders)
            {
                Vector3 localPosition = pieceCollider.transform.localPosition;
                minY = Mathf.Min(minY, localPosition.y);
                minX = Mathf.Min(minX, localPosition.x);
                maxY = Mathf.Max(maxY, localPosition.y);
                maxX = Mathf.Max(maxX, localPosition.x);
            }

            float offsetX = (minX);
            float offsetY = minY;
            int sizeX = Mathf.RoundToInt(maxX - minX) + 1;
            int sizeY = Mathf.RoundToInt(maxY - minY) + 1;

            // Normalize the piece positions by considering min values as offsets
            foreach (var pieceCollider in _piece.Colliders)
            {
                Vector3 localPosition = pieceCollider.transform.localPosition;
                int x = Mathf.RoundToInt(localPosition.x - offsetX);
                int y = Mathf.RoundToInt(localPosition.y - offsetY);

                // Ensure index is valid
                if (x >= 0 && x < sizeX && y >= 0 && y < sizeY)
                {
                    _cellMatrix[x, sizeY - y - 1] = true;
                }
            }
        }

        private void DrawCells()
        {
            float screenWidth = EditorGUIUtility.currentViewWidth;
            int cellSize = (int)((screenWidth - 90) / _cellMatrix.GetLength(1));
            cellSize = Mathf.Clamp(cellSize, MinCellSize, MaxCellSize);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Space(10);

            // Iterate through the rows and columns of the matrix.
            for (int i = 0; i < _cellMatrix.GetLength(0); i++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < _cellMatrix.GetLength(1); j++)
                {
                    if (j == 0)
                    {
                        GUILayout.FlexibleSpace();
                    }

                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                    if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    {
                        _cellMatrix[i, j] = !_cellMatrix[i, j];
                        Event.current.Use(); // Consume the event so other control does not use it
                        Repaint(); // Force window redrawing to update the display after changing values
                    }


                    Color color = _cellMatrix[i, j] ? Color.black : Color.white;
                    EditorGUI.DrawRect(rect, color);

                    if (j == _cellMatrix.GetLength(1) - 1)
                    {
                        GUILayout.FlexibleSpace();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }
    }
}