using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(GameManager.ModeLength))]
public class ArrayDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        contentPosition.width *= (float)1 / 3;
        float width = contentPosition.width;
        
        SerializedProperty values = property.FindPropertyRelative("value");

        for (int i = 0; i < 3; i++)
        {
            EditorGUI.PropertyField(contentPosition, values.GetArrayElementAtIndex(i), GUIContent.none);
            contentPosition.x += width;
        }

        EditorGUI.EndProperty();
    }
}
