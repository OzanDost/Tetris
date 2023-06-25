using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Game;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PieceCreator : OdinEditorWindow
    {
        private const string PiecePath = "Assets/Prefabs/Game/Pieces";
        private const string ShaderPath = "Assets/Shaders/Outline.shader";
        private const string MaterialPath = "Assets/Materials/";
        private const string PieceBaseName = "Piece_Base";

        private GameObject _basePiece;
        private GameObject _pieceToCreate;
        private Shader _basePieceShader;

        public string pieceName;

        [AssetSelector(Paths = "Assets/Sprites/Game/PieceCells")]
        public Sprite pieceCellSprite;

        public PieceType pieceType;

        [ColorPalette]
        public Color pieceColor;

        [BoxGroup]
        [TableMatrix(DrawElementMethod = nameof(DrawCell), SquareCells = true, ResizableColumns = false)]
        public bool[,] customCellDrawings = new bool[4, 4];

        private static readonly int s_OutlineColor = Shader.PropertyToID("_OutlineColor");

        public void Init()
        {
            if (_pieceToCreate != null)
            {
                DestroyImmediate(_pieceToCreate.gameObject);
            }

            _basePiece = AssetDatabase.LoadAssetAtPath<GameObject>($"{PiecePath}/{PieceBaseName}.prefab");
            _pieceToCreate = PrefabUtility.InstantiatePrefab(_basePiece) as GameObject;
            _basePieceShader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
        }

        private bool DrawCell(Rect rect, bool value)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                GUI.changed = true;
                Event.current.Use();
            }

            EditorGUI.DrawRect(rect.Padding(1), value ? Color.black : Color.white);
            return value;
        }


        [Button]
        public void SavePiece()
        {
            if (_pieceToCreate == null || _basePiece == null)
            {
                Init();
            }

            var dimensionSize = customCellDrawings.GetLength(0);
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int i = 0; i < dimensionSize; i++)
            {
                for (int j = 0; j < dimensionSize; j++)
                {
                    if (customCellDrawings[i, j])
                        positions.Add(new Vector2Int(i, j));
                }
            }

            //todo raise an error if there is a gap

            var middleX = (int)positions.Average(i => i.x);
            var middleY = (int)positions.Average(i => i.y);


            var averagePosition = new Vector2Int(middleX, middleY);

            var pieceComponent = _pieceToCreate.GetComponent<Piece>();
            Collider2D[] colliders = new Collider2D[positions.Count];
            SpriteRenderer[] spriteRenderers = new SpriteRenderer[positions.Count];
            var tempMaterial = new Material(_basePieceShader);
            
            AssetDatabase.CreateAsset(tempMaterial,$"{MaterialPath}{pieceName}.shader");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            var targetOutlineColor = pieceColor;

            targetOutlineColor.r -= 10;
            targetOutlineColor.g -= 10;
            targetOutlineColor.b -= 10;

            tempMaterial.SetColor(s_OutlineColor, targetOutlineColor);

            for (var i = 0; i < positions.Count; i++)
            {
                var cellPosition = positions[i];
                var go = new GameObject
                {
                    name = "PieceCell"
                };

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

            pieceComponent.SetReferencesFromEditor(colliders, pieceType);

            PrefabUtility.RecordPrefabInstancePropertyModifications(_pieceToCreate);
            
            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(_pieceToCreate, $"{PiecePath}/Piece_{pieceName}.prefab");

            EditorGUIUtility.PingObject(savedPrefab);

            DestroyImmediate(_pieceToCreate.gameObject);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}