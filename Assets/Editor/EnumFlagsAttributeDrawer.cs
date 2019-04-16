/*
 * 
 * This file contains an editor tool for drawing in editor bitmasks.
 * The code is from: https://answers.unity.com/questions/486694/default-editor-enum-as-flags-.html
 * 
 */

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer {
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
        _property.intValue = EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
    }
}
