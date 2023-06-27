using Enums;
using UnityEditor;

namespace Editor
{
    public static class EditorPrefHelper
    {
        private const string AddedNewPieceType = "AddedNewPieceType";
        private const string NewPieceType = "NewPieceType";

        private static void ToggleAddedNewPieceType(bool toggle)
        {
            EditorPrefs.SetBool(AddedNewPieceType, toggle);
        }

        public static bool WasNewPieceTypeAdded()
        {
            var addedNewPieceType = EditorPrefs.GetBool(AddedNewPieceType, false);
            return addedNewPieceType;
        }

        public static void SetNewPieceType(string pieceType)
        {
            ToggleAddedNewPieceType(true);
            EditorPrefs.SetString(NewPieceType, pieceType);
        }

        public static PieceType GetAddedNewPieceType()
        {
            ToggleAddedNewPieceType(false);
            var newPieceType = EditorPrefs.GetString(NewPieceType, "");
            var pieceType = (PieceType)System.Enum.Parse(typeof(PieceType), newPieceType);
            return pieceType;
        }
    }
}