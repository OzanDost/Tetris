using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using DefaultNamespace.Data;
using Enums;
using Game;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public class PieceCreator : OdinEditorWindow
    {
        private const string PiecePath = "Assets/Prefabs/Game/Pieces";
        private const string ShaderPath = "Assets/Shaders/SpriteOutline.shader";
        private const string MaterialPath = "Assets/Materials/";
        private const string PieceBaseName = "Piece_Base";
        private const string PieceTypesEnumFilePath = "Assets/Scripts/Enums/PieceType.cs";
        private const string PoolConfigFilePath = "PoolConfig";
        private const string GameConfigFilePath = "GameConfig";

        [SerializeField, BoxGroup("Create New Piece Type")]
        private string _newPieceEnum;

        [SerializeField, BoxGroup("Create New Piece")]
        private string _pieceName;

        [SerializeField, BoxGroup("Create New Piece")]
        private PieceType _pieceType;

        [SerializeField, BoxGroup("Create New Piece")]
        [AssetSelector(Paths = "Assets/Sprites/Game/PieceCells")]
        [ShowInInspector]
        private Sprite _pieceCellSprite;

        [SerializeField, BoxGroup("Create New Piece")]
        [ColorPalette]
        [ShowInInspector]
        private Color _pieceColor;

        [SerializeField, BoxGroup("Create New Piece")]
        [PropertySpace(spaceBefore: 10)]
        [TableMatrix(DrawElementMethod = nameof(DrawCell), SquareCells = true, ResizableColumns = false,
            HideColumnIndices = true, HideRowIndices = true)]
        [ShowInInspector]
        private bool[,] _cellMatrix = new bool[4, 4];

        private GameObject _basePiece;
        private GameObject _pieceToCreate;
        private Shader _basePieceShader;
        private GameConfig _gameConfig;
        private PoolConfig _poolConfig;
        private Material _tempMaterial;


        private bool DrawCell(Rect rect, bool value)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                GUI.changed = true;
                Event.current.Use();
            }

            var color = value ? Color.black : Color.white;
            EditorGUI.DrawRect(rect.Padding(1), color);
            return value;
        }

        private void ClearPiecesInScene()
        {
            var pieces = FindObjectsOfType<Piece>(true);
            foreach (var piece in pieces)
            {
                DestroyImmediate(piece.gameObject);
            }
        }

        [Button, BoxGroup("Create New Piece Type")]
        private void AddPieceType()
        {
            var existingEnums = File.ReadAllLines(PieceTypesEnumFilePath).ToList();

            if (existingEnums.Exists(existingEnum => existingEnum.Contains(_newPieceEnum)))
            {
                RaiseWarning("Piece type already exists!");
                return;
            }

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


        public void Init(TetrisEditor tetrisEditor)
        {
            ClearPiecesInScene();

            _basePiece = AssetDatabase.LoadAssetAtPath<GameObject>($"{PiecePath}/{PieceBaseName}.prefab");
            _pieceToCreate = PrefabUtility.InstantiatePrefab(_basePiece) as GameObject;
            _basePieceShader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);

            tetrisEditor.OnClose -= OnWindowClosed;
            tetrisEditor.OnClose += OnWindowClosed;
        }

        private void OnWindowClosed()
        {
            ClearPiecesInScene();
        }

        [Button, BoxGroup("Create New Piece")]
        public void CreatePiece()
        {
            if (!ReadyToCreatePiece()) return;
            if (SameTypePieceExists())
            {
                RaiseWarning("Piece with same type already exists!");
                return;
            }

            var positions = Utils.GetOccupiedCellPositions(_cellMatrix);

            SetPieceMaterial();

            var spriteRenderers = CreatePieceCells(positions);
            var colliders = AddCollidersToPieceCells(spriteRenderers);

            SetPieceReferences(colliders, spriteRenderers);

            SavePiecePrefab();

            AddNewPieceToPoolConfig(_pieceToCreate.GetComponent<Piece>());
        }

        private bool ReadyToCreatePiece()
        {
            if (_pieceToCreate == null || _basePiece == null)
            {
                Initialize();
                return false;
            }

            if (_pieceCellSprite == null)
            {
                RaiseWarning("You haven't selected a piece cell sprite!");
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

        private void SavePiecePrefab()
        {
            var savedPrefab =
                PrefabUtility.SaveAsPrefabAsset(_pieceToCreate, $"{PiecePath}/{_pieceName}.prefab");

            EditorGUIUtility.PingObject(savedPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            ClearPiecesInScene();
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
    }
}