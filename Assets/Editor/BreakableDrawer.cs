using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Breakable))]
public class BreakableDrawer : PropertyDrawer
{
    private float propertyHeight = 0f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty canBreakProp    = property.FindPropertyRelative("canBreak");
        SerializedProperty breakInUseProp  = property.FindPropertyRelative("canBreakDuringUse");
        SerializedProperty intervalProp    = property.FindPropertyRelative("interval");
        SerializedProperty safetyTimeProp  = property.FindPropertyRelative("safetyTime");
        SerializedProperty breakChanceProp = property.FindPropertyRelative("breakChance");
        SerializedProperty repairQTEProp   = property.FindPropertyRelative("repairQTE");
        SerializedProperty holdableProp    = property.FindPropertyRelative("requiredHoldable");
        SerializedProperty vfxBreakProp    = property.FindPropertyRelative("vfxBreakPrefab");
        SerializedProperty requireHoldable = property.FindPropertyRelative("requireHoldable");
        SerializedProperty vfxAnchorProp   = property.FindPropertyRelative("vfxAnchor");

        // Calculate rects
        Rect canBreakRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        float totalHeight = EditorGUIUtility.singleLineHeight + 2;

        // Draw can break field
        EditorGUI.PropertyField(canBreakRect, canBreakProp);

        // Update height and exit if canBreak is false
        if(canBreakProp.boolValue == false)
        {
            propertyHeight = totalHeight;
            EditorGUI.EndProperty();
            return;
        }

        // Otherwise, update height and draw the rest of the fields
        Rect breakInUseRect  = new(position.x, position.y + totalHeight, position.width, EditorGUIUtility.singleLineHeight);
        totalHeight += EditorGUIUtility.singleLineHeight + 2;

        Rect intervalRect    = new(position.x, position.y + totalHeight, position.width, EditorGUI.GetPropertyHeight(intervalProp));
        totalHeight += EditorGUI.GetPropertyHeight(intervalProp) + 2;

        Rect safetyTimeRect  = new(position.x, position.y + totalHeight, position.width, EditorGUI.GetPropertyHeight(safetyTimeProp));
        totalHeight += EditorGUI.GetPropertyHeight(safetyTimeProp) + 2;

        Rect breakChanceRect = new(position.x, position.y + totalHeight, position.width, EditorGUI.GetPropertyHeight(breakChanceProp));
        totalHeight += EditorGUI.GetPropertyHeight(breakChanceProp) + 2;

        Rect repairQTERect   = new(position.x, position.y + totalHeight, position.width, EditorGUI.GetPropertyHeight(repairQTEProp));
        totalHeight += EditorGUI.GetPropertyHeight(repairQTEProp) + 2;
        
        Rect holdableRect    = new(position.x, position.y + totalHeight, position.width, EditorGUI.GetPropertyHeight(holdableProp));
        totalHeight += EditorGUI.GetPropertyHeight(holdableProp) + 2;
        
        Rect vfxBreakRect    = new(position.x, position.y + totalHeight, position.width, EditorGUI.GetPropertyHeight(vfxBreakProp));
        totalHeight += EditorGUI.GetPropertyHeight(vfxBreakProp) + 2;

        Rect reqHoldRect   = new(position.x, position.y + totalHeight, position.width, EditorGUI.GetPropertyHeight(requireHoldable));
        totalHeight += EditorGUI.GetPropertyHeight(requireHoldable) + 2;

        Rect vfxAnchorRect   = new(position.x, position.y + totalHeight, position.width, EditorGUI.GetPropertyHeight(vfxAnchorProp));
        totalHeight += EditorGUI.GetPropertyHeight(vfxAnchorProp) + 2;



        propertyHeight = totalHeight;

        // Draw other fields
        EditorGUI.indentLevel++;

        EditorGUI.PropertyField(breakInUseRect, breakInUseProp);
        EditorGUI.PropertyField(intervalRect, intervalProp);
        EditorGUI.PropertyField(safetyTimeRect, safetyTimeProp);
        EditorGUI.PropertyField(breakChanceRect, breakChanceProp);
        EditorGUI.PropertyField(repairQTERect, repairQTEProp);
        EditorGUI.PropertyField(holdableRect, holdableProp);
        EditorGUI.PropertyField(vfxBreakRect, vfxBreakProp);
        EditorGUI.PropertyField(reqHoldRect, requireHoldable);
        EditorGUI.PropertyField(vfxAnchorRect, vfxAnchorProp);

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propertyHeight;
    }
}