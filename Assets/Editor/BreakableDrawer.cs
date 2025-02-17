using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Breakable))]
public class BreakableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        Rect canBreakRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect intervalRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect safetyTimeRect = new Rect(position.x, position.y + 2 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
        Rect breakChanceRect = new Rect(position.x, position.y + 3 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
        Rect repairQTERect = new Rect(position.x, position.y + 4 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
        Rect holdableRect = new Rect(position.x, position.y + 5 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);
        Rect vfxRect = new Rect(position.x, position.y + 6 * (EditorGUIUtility.singleLineHeight + 2), position.width, EditorGUIUtility.singleLineHeight);

        // Draw fields
        SerializedProperty canBreakProp = property.FindPropertyRelative("canBreak");
        EditorGUI.PropertyField(canBreakRect, canBreakProp);

        if (canBreakProp.boolValue)
        {
            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(intervalRect, property.FindPropertyRelative("interval"));
            EditorGUI.PropertyField(safetyTimeRect, property.FindPropertyRelative("safetyTime"));
            EditorGUI.PropertyField(breakChanceRect, property.FindPropertyRelative("breakChance"));
            EditorGUI.PropertyField(repairQTERect, property.FindPropertyRelative("repairQTE"));
            EditorGUI.PropertyField(holdableRect, property.FindPropertyRelative("holdable"));
            EditorGUI.PropertyField(vfxRect, property.FindPropertyRelative("vfx"));

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty canBreakProp = property.FindPropertyRelative("canBreak");
        int lineCount = canBreakProp.boolValue ? 7 : 1;
        return lineCount * (EditorGUIUtility.singleLineHeight + 2);
    }
}