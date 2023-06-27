using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TetrisEditor : OdinMenuEditorWindow
    {
        private static PieceCreator _pieceCreator;
        private static PieceEditor _pieceEditor;
        private static PoolConfigEditor _poolConfigEditor;
        private static GameConfigEditor _gameConfigEditor;

        [MenuItem("Tools/Tetris Editor")]
        private static void OpenWindow()
        {
            var window = GetWindow<TetrisEditor>();
            window.Show();
        }


        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            if (_pieceCreator == null)
            {
                _pieceCreator = CreateInstance<PieceCreator>();
            }

            _pieceCreator.Init(this);
            tree.Add("Piece Creator", _pieceCreator);

            if (_pieceEditor == null)
            {
                _pieceEditor = CreateInstance<PieceEditor>();
            }

            _pieceEditor.Init();
            tree.Add("Piece Editor", _pieceEditor);

            if (_poolConfigEditor == null)
            {
                _poolConfigEditor = CreateInstance<PoolConfigEditor>();
            }

            _poolConfigEditor.Init();
            tree.Add("Pool Config Editor", _poolConfigEditor);

            if (_gameConfigEditor == null)
            {
                _gameConfigEditor = CreateInstance<GameConfigEditor>();
            }

            _gameConfigEditor.Init();
            tree.Add("Game Config Editor", _gameConfigEditor);


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
                            _pieceCreator.Init(this);
                        }
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}