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
    public class PieceEditor : OdinEditorWindow
    {
        private const string PiecePath = "Assets/Prefabs/Game/Pieces";

        [AssetSelector(Paths = PiecePath, DisableListAddButtonBehaviour = true)]
        [SerializeField]
        [OnValueChanged("OnPieceChanged")]
        [PreviewField(Height = 100, Alignment = ObjectFieldAlignment.Left)]
        private GameObject _pieceToEdit;

        private Piece _piece;

        [SerializeField]
        [ShowIf("@_pieceToEdit != null")]
        [ReadOnly]
        private PieceType _pieceType;

        [SerializeField]
        [ShowIf("@_pieceToEdit != null")]
        private string _pieceName;

        [SerializeField]
        [ShowIf("@_pieceToEdit != null")]
        private Sprite _pieceCellSprite;

        [SerializeField]
        [ShowIf("@_pieceToEdit != null")]
        private Color _pieceCellColor;

        [InlineEditor]
        [ShowIf("@_pieceToEdit != null")]
        [HideLabel]
        [SerializeField]
        [ReadOnly]
        private Material _pieceMaterial;

        [SerializeField]
        [ShowIf("@_pieceToEdit != null")]
        [PropertySpace(spaceBefore: 10)]
        [TableMatrix(DrawElementMethod = nameof(DrawCell), SquareCells = true, ResizableColumns = false,
            HideColumnIndices = true, HideRowIndices = true, RowHeight = 30)]
        [ShowInInspector]
        [ReadOnly]
        [BoxGroup()]
        private bool[,] _cellMatrix = new bool[4, 4];


        public void Init()
        {
        }

        [Button]
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

        private void OnPieceChanged()
        {
            if (_pieceToEdit == null) return;
            LoadPiece();
        }

        private bool DrawCell(Rect rect, bool value)
        {
            var color = value ? Color.black : Color.white;

            EditorGUI.DrawRect(rect.Padding(1), color);
            return value;
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
            _pieceMaterial = _piece.SpriteRenderers[0].sharedMaterial;
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
    }
}