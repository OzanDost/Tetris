using System.IO;
using Enums;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PieceEnumGenerator : MonoBehaviour, ISerializationCallbackReceiver
    {
        private const string SavedMaxPieceEnumValuePrefKey = "SavedMaxAudioEnumValue";
        private const string PieceEnumsFileName = "PieceType.cs";

        private int SavedMaxEnumValue
        {
            get => PlayerPrefs.GetInt(SavedMaxPieceEnumValuePrefKey);
            set => PlayerPrefs.SetInt(SavedMaxPieceEnumValuePrefKey, value);
        }

        [Sirenix.OdinInspector.ReadOnly]
        public string[] enumNames;

        private static string[] _enumNames;

#if UNITY_EDITOR

        private string AudioEnumsFilePath
        {
            get
            {
                var ms = MonoScript.FromMonoBehaviour(this);
                var assetPath = AssetDatabase.GetAssetPath(ms);
                var pieceEnumPath = Path.Combine(Path.GetDirectoryName(assetPath) ?? "Assets/Scripts/Audio/",
                    PieceEnumsFileName);
                return pieceEnumPath;
            }
        }


        [InitializeOnLoadMethod]
        public static void SerializeEnumNames()
        {
            _enumNames = System.Enum.GetNames(typeof(PieceType));
        }

        [PropertySpace(20)]
        [Button(ButtonSizes.Medium, Style = ButtonStyle.FoldoutButton, Expanded = true)]
        public void AddNewPieceType(params string[] keys)
        {
            // if (keys == null) return;
            //
            // enumNames = System.Enum.GetNames(typeof(PieceType));
            //
            // var lines = GetAudioEnumLines();
            // var audioManager = GetComponent<AudioManager>();
            // for (int j = 0; j < keys.Length; j++)
            // {
            //     var key = keys[j];
            //
            //     key = key.RemoveWhitespace();
            //     
            //     if (enumNames.Contains(key))
            //     {
            //         continue;
            //     }
            //
            //     var enumValue = MaxEnumValue() + j + 1;
            //     for (var i = 0; i < lines.Count; i++)
            //     {
            //         var line = lines[i];
            //         if (line.Contains(((PieceType)MaxExistingEnumValue()).ToString()))
            //         {
            //             lines.Insert(i + j + 1, $"\t\t{key} = {enumValue},");
            //             break;
            //         }
            //     }
            //
            //     if (audioManager != null)
            //     {
            //         audioManager.audioDict.Add((PieceType)enumValue, new AudioData(null, 1f));
            //     }
            // }
            //
            // WriteAudioEnumFile(lines);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }

#endif
    }
}