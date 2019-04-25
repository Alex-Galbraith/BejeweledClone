using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// <para>This class allows us to create a scriptable object that allows us to drag and drop a float as a reference.</para>
/// <para>There is also a NotifyChange event that can be subscribed to which is called when the value is modified.</para>
/// This technique is explained here: https://www.youtube.com/watch?v=raQ3iHhE_Kk
/// </summary>
[CreateAssetMenu(fileName ="FloatReference", menuName ="Data Containers/Create Float Ref")]
public class FloatReference : ScriptableObject
{
    public event OnChange NotifyChange;
    public delegate void OnChange(float from, float to);

    [SerializeField]
    private float _value;
    public float Value {
        get {
            return _value;
        }
        set {
            NotifyChange?.Invoke(_value, value);
            _value = value;
        }
    }

    public static float operator +(FloatReference a, float b) {
        return a.Value + b;
    }

    public static float operator +(FloatReference a, FloatReference b) {
        return a.Value + b.Value;
    }

    public static float operator +(float a, FloatReference b) {
        return b.Value + a;
    }
}



//only generate editor files if we are in the editor.
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : PropertyDrawer {
    private static float extraHeight = 15;
    private static float paddingTop = 2;
    private static float paddingBot = 2;
    private static float indent = 10;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float height = base.GetPropertyHeight(property, label) + extraHeight + paddingBot + paddingTop;
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),
                              new GUIContent(label.text));

        var region = new Rect(position.x, position.y, position.width - 10, position.height - (extraHeight + paddingBot + paddingTop));
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
        var newValue = EditorGUI.FloatField(region, "Value", valueProperty.floatValue);
        if (EditorGUI.EndChangeCheck()) {
            valueProperty.floatValue = newValue;
            //Apply changes to ScriptableObject
            so.ApplyModifiedProperties();
        }
        EditorGUI.EndProperty();
    }
}
#endif