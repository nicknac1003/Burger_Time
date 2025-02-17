using UnityEngine;
using static EaseFunctions;

[System.Serializable]
public class SliderQTE : QuickTimeEvent
{
    [SerializeField] private GameObject sliderBar; // prefab for slider bar for instantiation
    [SerializeField] private GameObject sliderArrow; // prefab for slider arrow for instantiation

    private GameObject sliderBarInstance; // instance of slider bar
    private GameObject sliderArrowInstance; // instance of slider arrow

    [SerializeField] private int sliderTargetStart = 45; // pixel position along slider from 0 to 59
    [SerializeField] private int sliderTargetEnd   = 55; // pixel position along slider from 1 to 60
    [SerializeField] private float sliderTime      = 3f; // Speed slider moves along bar
    [SerializeField] private Ease easeIn; // Ease function slider moves with
    [SerializeField] private Ease easeOut;

    private float timer = -1f;
    private Vector3 arrowStartPosition;

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
            return 0f;
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
            return 1 - Mathf.Abs(arrowPosition - targetPosition) / targetRange;
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

    private void CreateUI(Transform parent)
    {
        // Instantiate UI Elements
        sliderBarInstance = GameObject.Instantiate(sliderBar, parent);
        sliderBarInstance.transform.localPosition = new Vector3(0f, 2.25f, 0f);

        Vector2 barSpriteSize   = sliderBar.GetComponent<SpriteRenderer>().sprite.rect.size;
        Vector2 arrowSpriteSize = sliderArrow.GetComponent<SpriteRenderer>().sprite.rect.size;

        sliderArrowInstance = GameObject.Instantiate(sliderArrow, sliderBarInstance.transform);
        sliderArrowInstance.transform.position = sliderBarInstance.transform.position + new Vector3(GlobalConstants.pixelWorldSize * -30, GlobalConstants.pixelWorldSize * ((barSpriteSize.y + arrowSpriteSize.y) / 2f - 2f) * GlobalConstants.yDistortion, 0f);
        arrowStartPosition = sliderArrowInstance.transform.position;

        // Update Shader
        sliderTargetStart = Mathf.Clamp(sliderTargetStart, 0, 59);
        sliderTargetEnd   = Mathf.Clamp(sliderTargetEnd,   1, 60);

        SpriteRenderer sliderSpriteRenderer = sliderBarInstance.GetComponent<SpriteRenderer>();
        sliderSpriteRenderer.material = new Material(sliderSpriteRenderer.material);
        sliderSpriteRenderer.material.SetFloat("_targetStartPosition", sliderTargetStart);
        sliderSpriteRenderer.material.SetFloat("_targetEndPosition", sliderTargetEnd);
    }

    private void DestroyUI()
    {
        GameObject.Destroy(sliderBarInstance);
        GameObject.Destroy(sliderArrowInstance);

        sliderBarInstance   = null;
        sliderArrowInstance = null;
    }

    private void Finished()
    {
        DestroyUI();
        timer = -1f;
    }
}