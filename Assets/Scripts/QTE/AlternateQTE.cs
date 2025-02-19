using UnityEngine;

public class AlternateQTE : QuickTimeEvent
{
    [Range(3, 20)][SerializeField] private int mashCount = 10;

    private GameObject     fillBarInstance;
    private SpriteRenderer fillBarSpriteRenderer;

    private GameObject    keyZ;
    private GameObject    keyC;
    private UIKeyAnimator keyZAnimator;
    private UIKeyAnimator keyCAnimator;

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
        
        if(cPressed)
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

        fillBarSpriteRenderer.material.SetFloat("_progress", progress);

        keyZAnimator.ToggleKey(GlobalConstants.alternateAnimationTime);
        keyCAnimator.ToggleKey(GlobalConstants.alternateAnimationTime);

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

        keyZ = new GameObject("KeyZ");
        keyZ.transform.parent = fillBarInstance.transform;
        keyZ.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
        keyZ.transform.localScale = new Vector3(1f, 1f, 1f); // same as parent
        keyZAnimator = keyZ.AddComponent<UIKeyAnimator>();
        keyZAnimator.Init(GlobalConstants.keyZ, KeyState.Down);

        keyC = new GameObject("KeyC");
        keyC.transform.parent = fillBarInstance.transform;
        keyC.transform.localPosition = new Vector3(0.5f, 0f, 0f);
        keyC.transform.localScale = new Vector3(1f, 1f, 1f); // same as parent
        keyCAnimator = keyC.AddComponent<UIKeyAnimator>();
        keyCAnimator.Init(GlobalConstants.keyC, KeyState.Up);
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