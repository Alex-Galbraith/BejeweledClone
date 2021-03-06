﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// <para>This class allows us to create a scriptable object that allows us to drag and drop a Vector2Int as a reference.</para>
/// <para>There is also a NotifyChange event that can be subscribed to which is called when the value is modified.</para>
/// This technique is explained here: https://www.youtube.com/watch?v=raQ3iHhE_Kk
/// </summary>
[CreateAssetMenu(fileName ="Vector2IntReference", menuName ="Data Containers/Create Vector2Int Ref")]
public class Vector2IntReference : ScriptableObject
{
    public event OnChange NotifyChange;
    public delegate void OnChange(Vector2Int from, Vector2Int to);

    [SerializeField]
    private Vector2Int _value;
    public Vector2Int Value {
        get {
            return _value;
        }
        set {
            NotifyChange?.Invoke(_value, value);
            _value = value;
        }
    }

    public int x {
        get {
            return Value.x;
        }

        set {
            Value = new Vector2Int(value, _value.y);
        }
    }
    public int y {
        get {
            return Value.y;
        }

        set {
            Value = new Vector2Int(_value.x, value);
        }
    }

    public static Vector2Int operator +(Vector2IntReference a, Vector2Int b) {
        return a.Value + b;
    }

    public static Vector2Int operator +(Vector2IntReference a, Vector2IntReference b) {
        return a.Value + b.Value;
    }

    public static Vector2Int operator +(Vector2Int a, Vector2IntReference b) {
        return b.Value + a;
    }
}


//only generate editor files if we are in the editor.
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Vector2IntReference))]
public class Vector2IntReferenceDrawer : PropertyDrawer {
    private float vecHeight;
    private static float paddingTop = 2;
    private static float paddingBot = 2;
    private static float indent = 10;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (property.objectReferenceValue == null) {
            return base.GetPropertyHeight(property, label);
        }
        //Unity is weird and we have to do this to access properties on ScriptableObjects
        SerializedObject so = new SerializedObject(property.objectReferenceValue);

        var valueProperty = so.FindProperty("_value");
        if (valueProperty == null) {
            return base.GetPropertyHeight(property, label);
        }
        vecHeight = EditorGUI.GetPropertyHeight(valueProperty, label);
        float height = base.GetPropertyHeight(property, label) + vecHeight + paddingBot + paddingTop;
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),
                              new GUIContent(label.text));

        var region = new Rect(position.x, position.y, position.width - 10, position.height - (vecHeight + paddingBot + paddingTop));
        EditorGUI.ObjectField(region, property);

        if (property.objectReferenceValue == null) {
            return;
        }

        //Unity is weird and we have to do this to access properties on ScriptableObjects
        SerializedObject so = new SerializedObject(property.objectReferenceValue);

        var valueProperty = so.FindProperty("_value");
        if (valueProperty == null) {

            return;
        }

        float labelHeight = base.GetPropertyHeight(property, label);

        region = new Rect(position.x + indent, position.y + labelHeight + paddingTop, position.width - 10, position.height - labelHeight - paddingBot);

        EditorGUI.BeginChangeCheck();
        var newValue = EditorGUI.Vector2IntField(region, "Value", valueProperty.vector2IntValue);
        if (EditorGUI.EndChangeCheck()) {
            valueProperty.vector2IntValue = newValue;
            //Apply changes to ScriptableObject
            so.ApplyModifiedProperties();
        }
        EditorGUI.EndProperty();
    }
}
#endif