using UnityEngine;

public class GlobalConstants
{
    public static float pixelsPerUnit  = 32f;
    public static float pixelWorldSize = 1f / pixelsPerUnit;
    public static float yDistortion    = Mathf.Sqrt(2);

    public static Color goodColor = new(0.5f, 1.0f, 0.6f);
    public static Color badColor  = new(1.0f, 0.4f, 0.4f);

    public static GameObject circleFill    = Resources.Load<GameObject>("QTE/Circle");
    public static GameObject sliderBar     = Resources.Load<GameObject>("QTE/Slider");
    public static GameObject sliderArrow   = Resources.Load<GameObject>("QTE/Arrow");
    public static GameObject boxFill       = Resources.Load<GameObject>("QTE/Box");
    public static GameObject alternateFill = Resources.Load<GameObject>("QTE/Alternate");
    public static GameObject diamondFill   = Resources.Load<GameObject>("QTE/Diamond");
}