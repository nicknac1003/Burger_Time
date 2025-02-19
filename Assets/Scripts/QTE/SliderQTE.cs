using Unity.VisualScripting;
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

    private float targetPosition;
    private float targetRange;
    private float ratio;
    private float timeToReachTargetFromStart;
    private float timeToReachEndFromTarget;
    private float arrowPosition;

    private void ResetTimer() => timer = -1f;
    private void StartTimer() => timer = 0f;

    public SliderQTE() : base(true)
    {
        sliderTargetStart = 45;
        sliderTargetEnd   = 55;
        sliderTime        = 1.5f;
        easeIn            = Ease.InCubic;
        easeOut           = Ease.OutCubic;
    }
    public SliderQTE(int start, int end, float time, Ease inEase, Ease outEase) : base(true)
    {
        sliderTargetStart = Mathf.Clamp(start, 0, 59);
        sliderTargetEnd   = Mathf.Clamp(end,   1, 60);
        sliderTime        = Mathf.Clamp(time, 0.1f, 2f);
        easeIn            = inEase;
        easeOut           = outEase;
    }

    public void SetTargetPosition(int start, int end)
    {
        if(InProgress()) return;

        sliderTargetStart = Mathf.Clamp(start, 0, 59);
        sliderTargetEnd   = Mathf.Clamp(end,   1, 60);
    }
    public void SetSliderTime(float time)
    {
        if(InProgress()) return;

        sliderTime = Mathf.Clamp(time, 0.1f, 2f);
    }

    public override bool InProgress()
    {
        return timer < sliderTime && timer >= 0f;
    }

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        if(xPressed)
        {
            EndQTE(parent);
            return 0f;
        }

        if(zPressed)
        {
            if(timer < 0f)
            {
                StartQTE(parent);
            }
            else
            {
                EndQTE(parent);
                return Mathf.Clamp01(1 - Mathf.Abs(arrowPosition - targetPosition) / targetRange);
            }

            return 0f;
        }
        
        if(InProgress() == false)
        {
            EndQTE(parent);
            return 0f;
        }

        arrowPosition = timer < timeToReachTargetFromStart ? EaseFunctions.Interpolate(0, targetPosition, timer / timeToReachTargetFromStart, easeIn) : EaseFunctions.Interpolate(targetPosition, 60, (timer - timeToReachTargetFromStart) / timeToReachEndFromTarget, easeOut);

        sliderArrowInstance.transform.position = arrowStartPosition + new Vector3(GlobalConstants.pixelWorldSize * arrowPosition, 0f, 0f);

        timer += Time.deltaTime;

        return 0f;
    }

    protected override void CreateUI(Transform parent)
    {
        DestroyUI();

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
    protected override void DestroyUI()
    {
        Object.Destroy(sliderBarInstance);
        Object.Destroy(sliderArrowInstance);

        sliderBarInstance   = null;
        sliderArrowInstance = null;
    }

    protected override void StartQTE(Interactable parent)
    {
        parent.ResetInteracts();
        StartTimer();
        CreateUI(parent.transform);

        targetPosition = (sliderTargetStart + sliderTargetEnd) / 2f;
        targetRange    = (sliderTargetEnd - sliderTargetStart) / 2f; // technically half the range
        ratio          = targetPosition / 60f;
        timeToReachTargetFromStart = sliderTime * ratio;
        timeToReachEndFromTarget   = sliderTime - timeToReachTargetFromStart;
        arrowPosition  = 0f;
    }
    public override void EndQTE(Interactable parent)
    {
        parent.ResetInteracts();
        ResetTimer();
        DestroyUI();
    }
}