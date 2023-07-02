using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using Enums;
using Game;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public class PieceCreator : EditorWindow
    {
        private const string PiecePath = "Assets/Prefabs/Game/Pieces";
        private const string ShaderPath = "Assets/Shaders/SpriteOutline.shader";
        private const string MaterialPath = "Assets/Materials/";
        private const string PieceBaseName = "Piece_Base";
        private const string PieceTypesEnumFilePath = "Assets/Scripts/Enums/PieceType.cs";
        private const string PoolConfigFilePath = "PoolConfig";
        private const string GameConfigFilePath = "GameConfig";

        private string _newPieceEnum;
        private string _pieceName;
        private PieceType _pieceType;
        private Sprite _pieceCellSprite;
        private Color _pieceColor;
        private readonly bool[,] _cellMatrix = new bool[4, 4];

        private GameObject _basePiece;
        private GameObject _pieceToCreate;
        private Shader _basePieceShader;
        private GameConfig _gameConfig;
        private PoolConfig _poolConfig;
        private Material _tempMaterial;
        private bool _isInitialized;


        private void ClearPiecesInScene()
        {
            var pieces = FindObjectsOfType<Piece>(true);
            foreach (var piece in pieces)
            {
                DestroyImmediate(piece.gameObject);
            }
        }

        private void AddPieceType()
        {
            var existingEnums = File.ReadAllLines(PieceTypesEnumFilePath).ToList();

            int index = existingEnums
                .FindLastIndex(line => !line.Contains("}"));

            existingEnums.Insert(index + 1, $"{_newPieceEnum} = {EditorPrefHelper.GetNewTypeNumber()},");
            File.WriteAllLines(PieceTypesEnumFilePath, existingEnums);

            EditorPrefHelper.SetNewPieceType(_newPieceEnum);
            AssetDatabase.Refresh();
        }


        private static void AddNewPieceToPoolConfig(Piece newPiece)
        {
            var poolConfig = Resources.Load<PoolConfig>(PoolConfigFilePath);
            poolConfig.AddItem(new PoolableItemData(newPiece, 50));
            AddNewPieceTypeToWeights();
            EditorUtility.SetDirty(poolConfig);
        }

        [DidReloadScripts]
        private static void AddNewPieceTypeToWeights()
        {
            if (EditorPrefHelper.WasNewPieceTypeAdded())
            {
                var newPieceType = EditorPrefHelper.GetAddedNewPieceType();
                var gameConfig = Resources.Load<GameConfig>(GameConfigFilePath);
                gameConfig.PieceWeightsDictionary.TryAdd(newPieceType, 1);
                EditorUtility.SetDirty(gameConfig);
                AssetDatabase.SaveAssets();
            }
        }


        [MenuItem("Tools/Piece Creator")]
        public static void ShowWindow()
        {
            PieceCreator window = (PieceCreator)GetWindow(typeof(PieceCreator));
            window.Show();
        }


        private bool CanValidateNewEnum()
        {
            if (SameTypePieceExists(_newPieceEnum))
            {
                RaiseWarning("Piece type already exists!");
                return false;
            }

            if (string.IsNullOrEmpty(_newPieceEnum) || _newPieceEnum.Length < 3)
            {
                RaiseWarning("New PieceType enum should be at least 3 characters long!");
                return false;
            }

            return true;
        }

        private bool CanValidateNewPiece()
        {
            if (_pieceCellSprite == null)
            {
                RaiseWarning("You haven't selected a piece cell sprite!");
                return false;
            }

            if (SameTypePieceExists())
            {
                RaiseWarning("Piece with same type already exists!");
                return false;
            }

            return true;
        }


        private void OnGUI()
        {
            if (!_isInitialized || _pieceToCreate == null)
            {
                Init();
            }

            GUILayout.Space(10); // Add some space between the two boxes

            DrawNewPieceType();

            GUILayout.Space(10); // Add some space between the two boxes

            DrawNewPiece();
        }

        private void DrawNewPiece()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            _pieceName = EditorGUILayout.TextField("Piece Name", _pieceName);
            _pieceType = (PieceType)EditorGUILayout.EnumPopup("Piece Type", _pieceType);
            _pieceColor = EditorGUILayout.ColorField("Piece Color", _pieceColor);

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            _pieceCellSprite = (Sprite)EditorGUILayout.ObjectField(_pieceCellSprite, typeof(Sprite), false,
                GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MaxHeight(85),
                GUILayout.MaxWidth(85));
            GUILayout.Label("Piece Cell Sprite");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();


            DrawCells();

            if (GUILayout.Button("Create Piece"))
            {
                CreatePiece();
            }

            GUILayout.EndVertical();
        }

        private void DrawNewPieceType()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Create New Piece Type", EditorStyles.boldLabel);
            _newPieceEnum = EditorGUILayout.TextField("New Piece Enum", _newPieceEnum);
            if (GUILayout.Button("Create Piece Type"))
            {
                if (!CanValidateNewEnum()) return;

                AddPieceType();
            }

            GUILayout.EndVertical();
        }

        private void DrawCells()
        {
            float screenWidth = EditorGUIUtility.currentViewWidth;
            int cellSize = (int)((screenWidth - 90) / _cellMatrix.GetLength(1));

            EditorGUILayout.BeginVertical();

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

        private void Init()
        {
            ClearPiecesInScene();

            _basePiece = AssetDatabase.LoadAssetAtPath<GameObject>($"{PiecePath}/{PieceBaseName}.prefab");
            _pieceToCreate = PrefabUtility.InstantiatePrefab(_basePiece) as GameObject;
            _basePieceShader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);

            _isInitialized = true;
        }

        private void OnDestroy()
        {
            ClearPiecesInScene();
        }

        private void CreatePiece()
        {
            if (!ReadyToCreatePiece()) return;

            if (!CanValidateNewPiece())
            {
                return;
            }

            var positions = Utils.GetOccupiedCellPositions(_cellMatrix);

            SetPieceMaterial();

            var spriteRenderers = CreatePieceCells(positions);
            var colliders = AddCollidersToPieceCells(spriteRenderers);

            SetPieceReferences(colliders, spriteRenderers);

            var savePiecePrefab = SavePiecePrefab();

            AddNewPieceToPoolConfig(savePiecePrefab.GetComponent<Piece>());

            ClearPieceComponents();
        }

        private bool ReadyToCreatePiece()
        {
            if (_pieceToCreate == null || _basePiece == null)
            {
                Init();
                return false;
            }

            return true;
        }

        private void SetPieceMaterial()
        {
            var targetOutlineColor = GetTargetOutlineColor();
            var tempMaterial = CreateNewMaterial();

            tempMaterial.SetColor("_SolidOutline", targetOutlineColor);

            AssetDatabase.CreateAsset(tempMaterial, $"{MaterialPath}{_pieceName}.mat");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private Color GetTargetOutlineColor()
        {
            var targetOutlineColor = _pieceColor;
            targetOutlineColor.r -= 40 / 255f;
            targetOutlineColor.g -= 40 / 255f;
            targetOutlineColor.b -= 40 / 255f;

            return targetOutlineColor;
        }

        private Material CreateNewMaterial()
        {
            _tempMaterial = new Material(_basePieceShader);
            return _tempMaterial;
        }

        private SpriteRenderer[] CreatePieceCells(List<Vector2Int> positions)
        {
            var spriteRenderers = new SpriteRenderer[positions.Count];

            for (var i = 0; i < positions.Count; i++)
            {
                var cellPosition = positions[i];
                var go = new GameObject { name = "PieceCell" };
                var spriteRenderer = go.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = _pieceCellSprite;
                spriteRenderer.color = _pieceColor;
                spriteRenderer.material = _tempMaterial;
                spriteRenderers[i] = spriteRenderer;
                go.transform.SetParent(_pieceToCreate.transform);
                var targetPosition = Utils.GetPivotPosition(positions, cellPosition);
                go.transform.localPosition = new Vector3(-targetPosition.x, targetPosition.y, 0);
            }

            return spriteRenderers;
        }

        private void ClearPieceComponents()
        {
            var componentCount = _pieceToCreate.transform.childCount;
            for (int i = 0; i < componentCount; i++)
            {
                DestroyImmediate(_pieceToCreate.transform.GetChild(0).gameObject);
            }
        }

        private Collider2D[] AddCollidersToPieceCells(SpriteRenderer[] spriteRenderers)
        {
            var colliders = new Collider2D[spriteRenderers.Length];

            for (var i = 0; i < spriteRenderers.Length; i++)
            {
                var collider = spriteRenderers[i].gameObject.AddComponent<BoxCollider2D>();
                colliders[i] = collider;
            }

            return colliders;
        }

        private void SetPieceReferences(Collider2D[] colliders, SpriteRenderer[] spriteRenderers)
        {
            var pieceComponent = _pieceToCreate.GetComponent<Piece>();
            pieceComponent.SetReferencesFromEditor(colliders, spriteRenderers, _pieceType);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_pieceToCreate);
        }

        private GameObject SavePiecePrefab()
        {
            var savedPrefab =
                PrefabUtility.SaveAsPrefabAsset(_pieceToCreate, $"{PiecePath}/{_pieceName}.prefab");

            EditorGUIUtility.PingObject(savedPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return savedPrefab;
        }

        private bool RaiseWarning(string message)
        {
            return EditorUtility.DisplayDialog("Warning", message, "Ok", "Cancel");
        }

        private bool SameTypePieceExists()
        {
            var existingPieces = AssetDatabase.FindAssets($"t:Prefab Piece_")
                .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(piece => piece != null)
                .Select(piece => piece.GetComponent<Piece>())
                .Where(pieceComponent => pieceComponent != null);

            foreach (var existingPiece in existingPieces)
            {
                if (existingPiece.PieceType == _pieceType)
                {
                    return true;
                }
            }

            return false;
        }

        private bool SameTypePieceExists(string pieceType)
        {
            var existingPieces = AssetDatabase.FindAssets($"t:Prefab Piece_")
                .Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(piece => piece != null)
                .Select(piece => piece.GetComponent<Piece>())
                .Where(pieceComponent => pieceComponent != null);

            foreach (var existingPiece in existingPieces)
            {
                var enumString = existingPiece.PieceType.ToString();
                if (enumString.Equals(pieceType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}