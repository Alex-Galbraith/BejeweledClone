
using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer {
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
        Enum targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, _property.intValue);
        Enum result =  EditorGUI.EnumFlagsField(_position, _label, targetEnum);
        _property.intValue = (int)Convert.ChangeType(result, targetEnum.GetType());
    }
}
