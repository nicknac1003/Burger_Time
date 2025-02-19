using UnityEngine;

public class AlternateQTE : QuickTimeEvent
{
    [Range(3, 20)][SerializeField] private int mashCount = 10;

    private GameObject     fillBarInstance;
    private SpriteRenderer fillBarSpriteRenderer;

    private bool zReleased = true;
    private bool cReleased = true;
    private bool zLastPressed = false;
    private int  mashProgress = 0;

    public AlternateQTE() : base(true) { mashCount = 10; }
    public AlternateQTE(int count) : base(true) { mashCount = count; }

    private void ResetMashing() => mashProgress = 0;

    public override bool InProgress()
    {
        return mashProgress < mashCount && mashProgress > 0;
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
            if(mashProgress == 0)
            {
                StartQTE(parent);
            }
            if(zReleased && zLastPressed == false)
            {
                zLastPressed = true;
                mashProgress++;
            }
        }
        else if(cPressed)
        {
            if(mashProgress == 0)
            {
                // Player is taking item out of Appliance before QTE starts
                return 0f;
            }
            if(cReleased && zLastPressed)
            {
                zLastPressed = false;
                mashProgress++;
            }
        }

        if(InProgress() == false)
        {
            float score = mashProgress >= mashCount ? 1f : 0f;
            EndQTE(parent);
            return score;
        }

        zReleased = !zPressed;
        cReleased = !cPressed;

        float progress = mashProgress / (float)mashCount;

        Debug.Log("Progress " + progress + " Z " + zPressed + " C " + cPressed + " ZR " + zReleased + " CR " + cReleased);

        fillBarSpriteRenderer.material.SetFloat("_progress", progress);

        return 0f;
    }

    protected override void CreateUI(Transform anchor)
    {
        DestroyUI();

        fillBarInstance = Object.Instantiate(GlobalConstants.alternateFill, anchor);
        fillBarInstance.transform.localPosition = new Vector3(0f, 2f, 0f);

        fillBarSpriteRenderer = fillBarInstance.GetComponent<SpriteRenderer>();
        fillBarSpriteRenderer.material = new Material(fillBarSpriteRenderer.material);
        fillBarSpriteRenderer.material.SetColor("_colorEmpty", GlobalConstants.badColor);
        fillBarSpriteRenderer.material.SetColor("_colorFilled", GlobalConstants.goodColor);
    }
    protected override void DestroyUI()
    {
        if(fillBarInstance == null) return;

        Object.Destroy(fillBarInstance);
        fillBarInstance       = null;
        fillBarSpriteRenderer = null;
    }

    protected override void StartQTE(Interactable parent)
    {
        CreateUI(parent.transform);
        zReleased    = true;
        cReleased    = true;
        zLastPressed = false;
    }
    public override void EndQTE(Interactable parent)
    {
        parent.ResetInteracts();
        ResetMashing();
        DestroyUI();
    }
}