using UnityEngine;

public class GlobalConstants
{
    public static float pixelsPerUnit  = 16f;
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

    public static UIKey keyBlank = Resources.Load<UIKey>("UIKeys/Blank");
    public static UIKey keyZ     = Resources.Load<UIKey>("UIKeys/Z");
    public static UIKey keyX     = Resources.Load<UIKey>("UIKeys/X");
    public static UIKey keyC     = Resources.Load<UIKey>("UIKeys/C");
    public static UIKey keySpace = Resources.Load<UIKey>("UIKeys/Space");
    public static UIKey keyUp    = Resources.Load<UIKey>("UIKeys/Up");
    public static UIKey keyDown  = Resources.Load<UIKey>("UIKeys/Down");
    public static UIKey keyRight = Resources.Load<UIKey>("UIKeys/Right");
    public static UIKey keyLeft  = Resources.Load<UIKey>("UIKeys/Left");
    public static UIKey keyEsc   = Resources.Load<UIKey>("UIKeys/Esc");

    public static IngredientObject bun     = Resources.Load<IngredientObject>("Ingredients/Bun");
    public static IngredientObject patty   = Resources.Load<IngredientObject>("Ingredients/Patty");
    public static IngredientObject lettuce = Resources.Load<IngredientObject>("Ingredients/Lettuce");
    public static IngredientObject tomato  = Resources.Load<IngredientObject>("Ingredients/Tomato");
    public static IngredientObject cheese  = Resources.Load<IngredientObject>("Ingredients/Cheese");
    public static IngredientObject onion   = Resources.Load<IngredientObject>("Ingredients/Onion");
    public static IngredientObject plate   = Resources.Load<IngredientObject>("Ingredients/Plate");

    public static GameObject speechBubble = Resources.Load<GameObject>("SpeechBubble");

    public static float alternateAnimationTime = 0.25f;
    public static float mashingAnimationTime   = 0.15f;
    public static float sliderAnimationTime    = 0.01f;
}