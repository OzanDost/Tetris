using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TetrisEditor : OdinMenuEditorWindow
    {
        public PieceCreator pieceCreator;

        [MenuItem("Tools/Tetris Editor")]
        private static void OpenWindow()
        {
            var window = GetWindow<TetrisEditor>();
            window.Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            if (pieceCreator == null)
            {
                pieceCreator = CreateInstance<PieceCreator>();
                pieceCreator.Init();
            }

            tree.Add("Piece Creator", pieceCreator);


            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();
                var selected = MenuTree.Selection.SelectedValue;
                if (MenuTree != null && MenuTree.Selection != null)
                {
                    if (selected is PieceCreator)
                    {
                        if (SirenixEditorGUI.ToolbarButton("Re-Init Piece Creator"))
                        {
                            pieceCreator.Init();
                        }
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}