using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(QuickTimeEvent), true)]
public class BaseClassDrawer : PropertyDrawer
{ 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    { 
        EditorGUI.BeginProperty(position, label, property); 
        var type = property.managedReferenceFullTypename; 
        var types = new[] { typeof(HoldQTE), typeof(RainbowQTE) }; 
        var typeNames = new[] { "None", "Hold", "Rainbow" };
        int index = 0; 
        if (!string.IsNullOrEmpty(type)) 
        { 
            for (int i = 0; i < types.Length; i++) 
            { 
                if (type.Contains(types[i].Name)) 
                { 
                    index = i + 1; 
                    break; 
                }
            }
        }

        index = EditorGUI.Popup(position, label.text, index, typeNames);
        
        if (index == 0) 
        {
            property.managedReferenceValue = null;
        }
        else
        {
            if (property.managedReferenceValue == null || property.managedReferenceFullTypename != types[index - 1].FullName)
            {
                property.managedReferenceValue = System.Activator.CreateInstance(types[index - 1]);
            }
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
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), childProperty, true);
                childProperty.NextVisible(false);
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
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
                childProperty.NextVisible(false);
            }
        }
        return height;
    }
}