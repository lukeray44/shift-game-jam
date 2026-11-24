using System.Collections;
using UnityEngine;
using System;


[Serializable]
/// Requires Unity 2020.1+
public struct Optional<T>
{
    [SerializeField] private bool enabled;
    [SerializeField] private T value;

    public bool Enabled => enabled;
    public T Value => value;
    public void Set(T value)
    {
        this.value = value;
        enabled = true;
    }
    public void SetActive(bool active)
    {
        enabled = active;
    }
    public Optional(T initialValue)
    {
        enabled = true;
        value = initialValue;
    }
}

#if UNITY_EDITOR
namespace UnityEditor
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label
        )
        {
            var valueProperty = property.FindPropertyRelative("value");
            var enabledProperty = property.FindPropertyRelative("enabled");

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;
            EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
            EditorGUI.PropertyField(position, valueProperty, label, true);
            EditorGUI.EndDisabledGroup();

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
            position.x -= position.width;
            EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
#endif