using UnityEngine;

public class GlobalConstants
{
    public static float pixelsPerUnit  = 32f;
    public static float pixelWorldSize = 1f / pixelsPerUnit;
    public static float yDistortion    = Mathf.Sqrt(2);

    public static Color goodColor = new(0.5f, 1.0f, 0.6f);
    public static Color badColor  = new(1.0f, 0.4f, 0.4f);
}