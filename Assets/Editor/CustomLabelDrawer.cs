using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CustomLabelAttribute))]
public class CustomLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var newLabel = attribute as CustomLabelAttribute;
        EditorGUI.PropertyField(position, property, new GUIContent(newLabel.Value), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}