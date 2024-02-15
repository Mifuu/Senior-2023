using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using PropertyAttributes;

[CustomPropertyDrawer(typeof(GenericMaskAttribute))]
public class GenericMaskPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attrib = this.attribute as GenericMaskAttribute;
        if (attrib == null)
        {
            base.OnGUI(position, property, label);
            return;
        }

        property.intValue = EditorGUI.MaskField(position, label, property.intValue, attrib.MaskNames);
    }
}
