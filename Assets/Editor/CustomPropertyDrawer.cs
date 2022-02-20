using UnityEngine;
using UnityEditor;

[UnityEditor.CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustomPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        
        SerializedProperty data = property.FindPropertyRelative("rows");
        SerializedProperty width = property.FindPropertyRelative("width");
        SerializedProperty height = property.FindPropertyRelative("height");
        data.arraySize = height.intValue;
        
        Rect newposition = position;
        newposition.y += height.intValue * 18f;

        for (int j = 0; j < data.arraySize; j++)
        {
            SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
            row.arraySize = width.intValue;
            
            newposition.height = 18f;

            newposition.width = position.width / row.arraySize;
            for (int i = 0; i < row.arraySize; i++)
            {
                EditorGUI.PropertyField(newposition, row.GetArrayElementAtIndex(i), GUIContent.none);
                newposition.x += newposition.width;
            }

            newposition.x = position.x;
            newposition.y -= 18f;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * 15;
    }
}