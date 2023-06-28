using System;
using Enums;
using UnityEditor;

public static class EditorPrefHelper
{
    private const string AddedNewPieceType = "AddedNewPieceType";
    private const string NewPieceType = "NewPieceType";
    private const string LastPieceTypeNumber = "LastPieceTypeNumber";
    private const string LastAmountOfPieces = "LastAmountOfPieces";
    private const string LastAmountOfPieceTypes = "LastAmountOfPieceTypes";

    private static void ToggleAddedNewPieceType(bool toggle)
    {
        EditorPrefs.SetBool(AddedNewPieceType, toggle);
    }

    public static void Test()
    {
    }

    public static int Testt()
    {
        return 0;
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

        var pieceTypeCount = Enum.GetValues(typeof(PieceType)).Length;
        SetLastAmountOfPieceTypes(pieceTypeCount);

        return pieceType;
    }

    public static int GetNewTypeNumber()
    {
        if (!EditorPrefs.HasKey(LastPieceTypeNumber))
        {
            var values = Enum.GetValues(typeof(PieceType));
            var biggest = 0;
            for (int i = 0; i < values.Length; i++)
            {
                var number = (int)values.GetValue(i);
                if (number > biggest)
                {
                    biggest = number;
                }
            }

            SetLastPieceTypeNumber(biggest + 1);
            return biggest + 1;
        }

        var lastPieceTypeNumber = EditorPrefs.GetInt(LastPieceTypeNumber, 0);
        SetLastPieceTypeNumber(lastPieceTypeNumber + 1);
        return lastPieceTypeNumber + 1;
    }

    private static void SetLastPieceTypeNumber(int number)
    {
        EditorPrefs.SetInt(LastPieceTypeNumber, number);
    }

    public static int GetLastAmountOfPieces()
    {
        if (!EditorPrefs.HasKey(LastAmountOfPieces))
        {
            SetLastAmountOfPieces(0);
            return 0;
        }

        var lastAmountOfPieces = EditorPrefs.GetInt(LastAmountOfPieces, 0);
        return lastAmountOfPieces;
    }

    private static void SetLastAmountOfPieces(int amount)
    {
        EditorPrefs.SetInt(LastAmountOfPieces, amount);
    }

    public static int GetLastAmountOfPieceTypes()
    {
        if (!EditorPrefs.HasKey(LastAmountOfPieceTypes))
        {
            SetLastAmountOfPieceTypes(0);
            return 0;
        }

        var lastAmountOfPieceTypes = EditorPrefs.GetInt(LastAmountOfPieceTypes, 0);
        return lastAmountOfPieceTypes;
    }

    public static void SetLastAmountOfPieceTypes(int amount)
    {
        EditorPrefs.SetInt(LastAmountOfPieceTypes, amount);
    }
}