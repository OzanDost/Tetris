using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CustomSquareMatrixDrawer : OdinValueDrawer<bool[,]>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            bool[,] value = this.ValueEntry.SmartValue;
            float screenWidth = EditorGUIUtility.currentViewWidth;
            int cellSize = (int)((screenWidth - 90) / value.GetLength(1));

            GUILayout.BeginVertical();
            if (label != null)
            {
                GUILayout.Label(label);
            }

            // Iterate through the rows and columns of the matrix.
            for (int i = 0; i < value.GetLength(0); i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < value.GetLength(1); j++)
                {
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(cellSize), GUILayout.Height(cellSize));

                    // Set the background color based on the cell value.
                    Color color = value[i, j] ? Color.black : Color.green;
                    EditorGUI.DrawRect(rect, color);
                    EditorGUI.BeginChangeCheck();

                    // Center the value inside the cell using the fontStyle.
                    EditorGUI.LabelField(rect, "", new GUIStyle()
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 20,
                        fontStyle = FontStyle.Bold,
                        normal = new GUIStyleState()
                        {
                            textColor = Color.white
                        }
                    });

                    if (EditorGUI.EndChangeCheck())
                    {
                        this.ValueEntry.SmartValue = value;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        // Method to determine the background color based on cell value
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.ValueEntry.TypeOfValue == typeof(int[,]);
        }
    }
}