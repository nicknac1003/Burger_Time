using UnityEngine;
using static EaseFunctions;

[System.Serializable]
public class SliderQTE : QuickTimeEvent
{
    [SerializeField] private GameObject sliderBar; // prefab for slider bar for instantiation
    [SerializeField] private GameObject sliderArrow; // prefab for slider arrow for instantiation

    private GameObject sliderBarInstance; // instance of slider bar
    private GameObject sliderArrowInstance; // instance of slider arrow

    [Tooltip("Pixel position along slider from 0 to 59.")]
    [Range(0, 59)][SerializeField] private int sliderTargetStart = 45;

    [Tooltip("Pixel position along slider from 1 to 60.")]
    [Range(1, 60)][SerializeField] private int sliderTargetEnd   = 55;
    
    [Tooltip("How long it takes for arrow to reach the end of the bar.")]
    [Range(0.1f, 2f)][SerializeField] private float sliderTime = 1.5f;

    [Tooltip("Ease function for arrow moving from start to target.")]
    [SerializeField] private Ease easeIn;

    [Tooltip("Ease function for arrow moving from target to end.")]
    [SerializeField] private Ease easeOut;

    private float timer = -1f;
    private Vector3 arrowStartPosition;

    public SliderQTE() // default constructor
    {
        sliderTargetStart = 45;
        sliderTargetEnd   = 55;
        sliderTime        = 1.5f;
        easeIn            = Ease.InCubic;
        easeOut           = Ease.OutCubic;
    }
    public SliderQTE(int start, int end, float time, Ease inEase, Ease outEase)
    {
        sliderTargetStart = Mathf.Clamp(start, 0, 59);
        sliderTargetEnd   = Mathf.Clamp(end,   1, 60);
        sliderTime        = Mathf.Clamp(time, 0.1f, 2f);
        easeIn            = inEase;
        easeOut           = outEase;
    }

    public void SetTargetPosition(int start, int end)
    {
        sliderTargetStart = Mathf.Clamp(start, 0, 59);
        sliderTargetEnd   = Mathf.Clamp(end,   1, 60);
    }
    public void SetSliderTime(float time) => sliderTime = Mathf.Clamp(time, 0.1f, 2f);

    public override bool InProgress()
    {
        return timer < sliderTime && timer >= 0f;
    }

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        if (timer < 0f)
        {
            // Did we press Z to initiate?
            if(zPressed == false) return 0f;

            timer = 0f;
            CreateUI(parent.transform);
            parent.ResetInteracts(); // Reset to prevent multiple interactions
            zPressed = false;
        }

        float targetPosition = (sliderTargetStart + sliderTargetEnd) / 2f;
        float targetRange    = (sliderTargetEnd - sliderTargetStart) / 2f; // technically half the range
        float ratio          = targetPosition / 60f;
        float timeToReachTargetFromStart = sliderTime * ratio;
        float timeToReachEndFromTarget   = sliderTime - timeToReachTargetFromStart;
        
        float arrowPosition = timer < timeToReachTargetFromStart ? EaseFunctions.Interpolate(0, targetPosition, timer / timeToReachTargetFromStart, easeIn) : EaseFunctions.Interpolate(targetPosition, 60, (timer - timeToReachTargetFromStart) / timeToReachEndFromTarget, easeOut);

        sliderArrowInstance.transform.position = arrowStartPosition + new Vector3(GlobalConstants.pixelWorldSize * arrowPosition, 0f, 0f);

        // Did we press Z to confirm?
        if(zPressed)
        {
            Finished();
            return Mathf.Clamp01(1 - Mathf.Abs(arrowPosition - targetPosition) / targetRange);
        }

        if(InProgress())
        {
            timer += Time.deltaTime;
        }
        else
        {
            Finished();
        }

        return 0f;
    }

    public override void CreateUI(Transform parent)
    {
        // Instantiate UI Elements
        sliderBarInstance = Object.Instantiate(sliderBar, parent);
        sliderBarInstance.transform.localPosition = new Vector3(0f, 2.25f, 0f);

        Vector2 barSpriteSize   = sliderBar.GetComponent<SpriteRenderer>().sprite.rect.size;
        Vector2 arrowSpriteSize = sliderArrow.GetComponent<SpriteRenderer>().sprite.rect.size;

        sliderArrowInstance = Object.Instantiate(sliderArrow, sliderBarInstance.transform);
        sliderArrowInstance.transform.position = sliderBarInstance.transform.position + new Vector3(GlobalConstants.pixelWorldSize * -30, GlobalConstants.pixelWorldSize * ((barSpriteSize.y + arrowSpriteSize.y) / 2f - 2f) * GlobalConstants.yDistortion, 0f);
        arrowStartPosition = sliderArrowInstance.transform.position;

        // Update Shader
        sliderTargetStart = Mathf.Clamp(sliderTargetStart, 0, 59);
        sliderTargetEnd   = Mathf.Clamp(sliderTargetEnd,   1, 60);

        SpriteRenderer sliderSpriteRenderer = sliderBarInstance.GetComponent<SpriteRenderer>();
        sliderSpriteRenderer.material = new Material(sliderSpriteRenderer.material);
        sliderSpriteRenderer.material.SetFloat("_targetStartPosition", sliderTargetStart);
        sliderSpriteRenderer.material.SetFloat("_targetEndPosition", sliderTargetEnd);
        sliderSpriteRenderer.material.SetColor("_targetColor", GlobalConstants.goodColor);
        sliderSpriteRenderer.material.SetColor("_failColor", GlobalConstants.badColor);
    }

    public override void DestroyUI()
    {
        Object.Destroy(sliderBarInstance);
        Object.Destroy(sliderArrowInstance);

        sliderBarInstance   = null;
        sliderArrowInstance = null;
    }

    private void Finished()
    {
        DestroyUI();
        timer = -1f;
    }
}