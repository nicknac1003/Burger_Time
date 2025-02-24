using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(QuickTimeEvent), true)]
public class QTEDrawer : PropertyDrawer
{ 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    { 
        EditorGUI.BeginProperty(position, label, property); 

        string type  = property.managedReferenceFullTypename;
        if(string.IsNullOrEmpty(type)) type = "HoldQTE"; // default value

        Type[] types = new[] { typeof(HoldQTE), typeof(SliderQTE), typeof(MashQTE), typeof(AlternateQTE), typeof(WindmillQTE) };
        string[] dropDownOptions = new[] {"Hold", "Slider", "Mash", "Alternate", "Windmill"};

        int index = 0; 
        for (int i = 0; i < types.Length; i++) 
        { 
            if (type.Contains(types[i].Name)) 
            { 
                index = i; 
                break; 
            }
        }

        EditorGUI.BeginChangeCheck();

        Rect popupPosition = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        index = EditorGUI.Popup(popupPosition, label.text, index, dropDownOptions);
        if(EditorGUI.EndChangeCheck())
        {
            property.serializedObject.Update();

            if (property.managedReferenceValue == null || property.managedReferenceFullTypename != types[index].FullName)
            {
                property.managedReferenceValue = System.Activator.CreateInstance(types[index]);
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        if(property.managedReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            var childProperty = property.Copy();
            var endProperty = property.GetEndProperty();
            childProperty.NextVisible(true);
            while (!SerializedProperty.EqualContents(childProperty, endProperty))
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                DrawProperty(ref position, childProperty);
                childProperty.NextVisible(false);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private void DrawProperty(ref Rect position, SerializedProperty property)
    {
        if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, true);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < property.arraySize; i++)
                {
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.GetArrayElementAtIndex(i), true);
                }
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;
        if (property.managedReferenceValue != null)
        {
            var childProperty = property.Copy();
            var endProperty = property.GetEndProperty();
            childProperty.NextVisible(true);
            while (!SerializedProperty.EqualContents(childProperty, endProperty))
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (childProperty.isArray && childProperty.propertyType == SerializedPropertyType.Generic && childProperty.isExpanded)
                {
                    for (int i = 0; i < childProperty.arraySize; i++)
                    {
                        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                childProperty.NextVisible(false);
            }
        }
        return height;
    }
}