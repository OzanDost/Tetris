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


        [PreviewField(100, ObjectFieldAlignment.Left), AssetsOnly,
         AssetSelector(Paths = "Assets/Prefabs/Game/Pieces"), BoxGroup("Edit Piece")]
        [OnValueChanged("OnPieceToEditChanged")]
        [InfoBox("Select a piece to edit, you can edit it or save it as a new piece by changing it's name and type.")]
        public GameObject pieceToEdit;

        [SerializeField, BoxGroup("Create New Piece Type")]
        private string _newPieceEnum;

        [SerializeField, BoxGroup("Create New Piece")]
        private string pieceName;

        [SerializeField, BoxGroup("Create New Piece")]
        private PieceType pieceType;

        [SerializeField, BoxGroup("Create New Piece")]
        [AssetSelector(Paths = "Assets/Sprites/Game/PieceCells")]
        [ShowInInspector]
        private Sprite pieceCellSprite;

        [SerializeField, BoxGroup("Create New Piece")]
        [ColorPalette]
        [ShowInInspector]
        private Color pieceColor;

        [SerializeField, BoxGroup("Create New Piece")]
        [PropertySpace(spaceBefore: 10)]
        [TableMatrix(DrawElementMethod = nameof(DrawCell), SquareCells = true, ResizableColumns = false,
            HideColumnIndices = true, HideRowIndices = true)]
        [ShowInInspector]
        private bool[,] customCellDrawings = new bool[4, 4];

        private GameObject _basePiece;
        private GameObject _pieceToCreate;
        private Shader _basePieceShader;
        private GameConfig _gameConfig;
        private PoolConfig _poolConfig;
        private bool _addedPiece;


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

            existingEnums.Insert(index, $"{_newPieceEnum},");
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

        private void OnPieceToEditChanged()
        {
            LoadPiece();
        }

        private void LoadPiece()
        {
            if (pieceToEdit == null)
            {
                return;
            }

            var piece = pieceToEdit.GetComponent<Piece>();
            if (piece == null)
            {
                return;
            }

            pieceName = piece.name;
            pieceType = piece.PieceType;
            pieceCellSprite = piece.SpriteRenderers[0].sprite;
            pieceColor = piece.SpriteRenderers[0].color;

            for (int i = 0; i < customCellDrawings.GetLength(0); i++)
            {
                for (int j = 0; j < customCellDrawings.GetLength(1); j++)
                {
                    customCellDrawings[i, j] = false;
                }
            }


            float minY = float.MaxValue;
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            float maxX = float.MinValue;

            // Calculate Min and Max positions
            foreach (var pieceCollider in piece.Colliders)
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
            foreach (var pieceCollider in piece.Colliders)
            {
                Vector3 localPosition = pieceCollider.transform.localPosition;
                int x = Mathf.RoundToInt(localPosition.x - offsetX);
                int y = Mathf.RoundToInt(localPosition.y - offsetY);

                // Ensure index is valid
                if (x >= 0 && x < sizeX && y >= 0 && y < sizeY)
                {
                    customCellDrawings[x, sizeY - y - 1] = true;
                }
            }
        }

        public void Init(TetrisEditor tetrisEditor)
        {
            ClearPiecesInScene();

            _basePiece = AssetDatabase.LoadAssetAtPath<GameObject>($"{PiecePath}/{PieceBaseName}.prefab");
            _pieceToCreate = PrefabUtility.InstantiatePrefab(_basePiece) as GameObject;
            _basePieceShader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
            _addedPiece = false;

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
            if (_pieceToCreate == null || _basePiece == null)
            {
                Initialize();
            }

            if (SameTypePieceExists())
            {
                if (pieceToEdit != null)
                {
                    if (!RaiseWarning("Would you like to overwrite the existing piece?"))
                    {
                        return;
                    }
                }
                else
                {
                    RaiseWarning("Piece with same type already exists!");
                    return;
                }
            }

            _addedPiece = true;

            var positions = Enumerable.Range(0, customCellDrawings.GetLength(0))
                .SelectMany(i => Enumerable.Range(0, customCellDrawings.GetLength(1))
                    .Where(j => customCellDrawings[i, j])
                    .Select(j => new Vector2Int(i, j)))
                .ToList();

            var middleX = (int)positions.Average(i => i.x);
            var middleY = (int)positions.Average(i => i.y);

            var averagePosition = new Vector2Int(middleX, middleY);

            var pieceComponent = _pieceToCreate.GetComponent<Piece>();
            var colliders = new Collider2D[positions.Count];
            var spriteRenderers = new SpriteRenderer[positions.Count];
            var tempMaterial = new Material(_basePieceShader);

            AssetDatabase.CreateAsset(tempMaterial, $"{MaterialPath}{pieceName}.mat");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var targetOutlineColor = pieceColor;
            targetOutlineColor.r -= 40 / 255f;
            targetOutlineColor.g -= 40 / 255f;
            targetOutlineColor.b -= 40 / 255f;

            tempMaterial.SetColor("_SolidOutline", targetOutlineColor);

            for (var i = 0; i < positions.Count; i++)
            {
                var cellPosition = positions[i];
                var go = new GameObject { name = "PieceCell" };
                var spriteRenderer = go.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = pieceCellSprite;
                spriteRenderer.color = pieceColor;
                spriteRenderer.material = tempMaterial;
                spriteRenderers[i] = spriteRenderer;
                var collider = go.AddComponent<BoxCollider2D>();
                colliders[i] = collider;
                go.transform.SetParent(_pieceToCreate.transform);
                var targetPosition = averagePosition - cellPosition;
                go.transform.localPosition = new Vector3(-targetPosition.x, targetPosition.y, 0);
            }

            pieceComponent.SetReferencesFromEditor(colliders, spriteRenderers, pieceType);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_pieceToCreate);

            var savedPrefab =
                PrefabUtility.SaveAsPrefabAsset(_pieceToCreate, $"{PiecePath}/{pieceName}.prefab");

            EditorGUIUtility.PingObject(savedPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AddNewPieceToPoolConfig(savedPrefab.GetComponent<Piece>());

            foreach (var spriteRenderer in spriteRenderers)
            {
                // DestroyImmediate(spriteRenderer.gameObject);
            }
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
            var existingPieces = Utils.GetSavedPieces();

            foreach (var existingPiece in existingPieces)
            {
                if (existingPiece.PieceType == pieceType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}