using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SpriteSlicer : EditorWindow
{
    // Simple container for animation range data.
    [System.Serializable]
    public class AnimationRange
    {
        public string animationName = "NewAnim";
        public int startFrame = 0;
        public int endFrame = 0;
    }

    // A list of ranges that you can customize in the window.
    public List<AnimationRange> animationRanges = new List<AnimationRange>()
    {
        new AnimationRange(){ animationName = "Idle", startFrame = 0, endFrame = 3 },
        new AnimationRange(){ animationName = "WalkLeft", startFrame = 4, endFrame = 9 },
        // Add additional default ranges as needed.
    };

    // Add menu item to open the window.
    [MenuItem("Tools/SpriteSheet Animation Creator")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSlicer>("SpriteSheet Animation Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Define Animation Ranges", EditorStyles.boldLabel);

        // List each animation range entry with fields to modify.
        for (int i = 0; i < animationRanges.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            animationRanges[i].animationName = EditorGUILayout.TextField("Name", animationRanges[i].animationName);
            animationRanges[i].startFrame = EditorGUILayout.IntField("Start Frame", animationRanges[i].startFrame);
            animationRanges[i].endFrame = EditorGUILayout.IntField("End Frame", animationRanges[i].endFrame);
            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                animationRanges.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        // Button to add a new animation range.
        if (GUILayout.Button("Add Animation Range"))
        {
            animationRanges.Add(new AnimationRange());
        }

        GUILayout.Space(20);

        // Process the selected sprite sheets.
        if (GUILayout.Button("Process Selected Sprite Sheets"))
        {
            ProcessSelectedSpriteSheets();
        }
    }

    /// <summary>
    /// Loops through each selected asset, checks that it's a sprite sheet (imported as Multiple),
    /// and creates an AnimationClip for each defined animation range.
    /// </summary>
    void ProcessSelectedSpriteSheets()
    {
        Object[] selectedObjects = Selection.objects;
        foreach (var obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null && importer.spriteImportMode == SpriteImportMode.Multiple)
            {
                // Load all sprites that were sliced from this texture.
                Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

                // For each animation range defined, create an AnimationClip.
                foreach (var range in animationRanges)
                {
                    CreateAnimationClip(assetPath, sprites, range.animationName, range.startFrame, range.endFrame);
                }
            }
            else
            {
                Debug.LogWarning("Selected asset is not a multi-sprite sheet: " + assetPath);
            }
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Creates an AnimationClip asset from a range of sprites.
    /// </summary>
    /// <param name="assetPath">Path to the sprite sheet asset.</param>
    /// <param name="sprites">Array of sliced sprites.</param>
    /// <param name="clipName">Name of the animation clip.</param>
    /// <param name="startFrame">Index of the starting frame (0-indexed).</param>
    /// <param name="endFrame">Index of the ending frame (inclusive, 0-indexed).</param>
    static void CreateAnimationClip(string assetPath, Object[] sprites, string clipName, int startFrame, int endFrame)
    {
        if (sprites.Length <= endFrame)
        {
            Debug.LogWarning($"Sprite sheet does not have enough frames for {clipName} (requested end frame: {endFrame}, available frames: {sprites.Length}).");
            return;
        }

        // Create a new AnimationClip and set its frame rate (adjust as necessary).
        AnimationClip clip = new AnimationClip { frameRate = 12f };

        // Create keyframes for the sprite property.
        int keyframeCount = endFrame - startFrame + 1;
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[keyframeCount];

        for (int i = startFrame; i <= endFrame; i++)
        {
            ObjectReferenceKeyframe keyframe = new ObjectReferenceKeyframe
            {
                // Time for each keyframe based on the clip's frame rate.
                time = (i - startFrame) / clip.frameRate,
                // Reference to the sprite for that frame.
                value = sprites[i]
            };
            keyframes[i - startFrame] = keyframe;
        }

        // Define the binding for the SpriteRenderer's sprite property.
        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        // Associate the keyframes with the clip.
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        // Save the clip as an asset in the same directory as the sprite sheet.
        string directory = Path.GetDirectoryName(assetPath);
        string fileName = Path.GetFileNameWithoutExtension(assetPath);
        string clipPath = Path.Combine(directory, $"{fileName}_{clipName}.anim");
        AssetDatabase.CreateAsset(clip, clipPath);

        Debug.Log($"Created AnimationClip: {clipPath}");
    }
}