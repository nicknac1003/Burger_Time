using UnityEngine;

public class MashQTE : QuickTimeEvent
{
    [Tooltip("The number of times the player must press Z to succeed.")]
    [Range(5, 30)][SerializeField] private int mashCount = 10;

    private GameObject fillBarInstance;
    private SpriteRenderer fillBarSpriteRenderer;

    private GameObject keyZ;
    private UIKeyAnimator keyZAnimator;

    private bool zReleased = true;
    private int mashProgress = -1;

    public MashQTE() : base(true) { mashCount = 10; }
    public MashQTE(int count) : base(true) { mashCount = count; }

    private void ResetMashing() => mashProgress = -1;
    private void StartMashing() => mashProgress = 0;

    public override bool InProgress()
    {
        return mashProgress < mashCount && mashProgress >= 0;
    }

    protected override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        if (xPressed)
        {
            EndQTE(parent);
            return 0f;
        }

        if (zPressed)
        {
            if (mashProgress < 0)
            {
                StartQTE(parent);
            }
            if (zReleased)
            {
                mashProgress++;
                PlayProgressSound();
            }
        }

        if (InProgress() == false)
        {
            float score = mashProgress >= mashCount ? 1f : 0f;
            EndQTE(parent);
            return score;
        }

        zReleased = !zPressed;

        float progress = mashProgress / (float)mashCount;

        fillBarSpriteRenderer.material.SetFloat("_progress", progress);

        keyZAnimator.ToggleKey(GlobalConstants.mashingAnimationTime);

        return 0f;
    }

    protected override void CreateUI(Transform anchor)
    {
        DestroyUI();

        fillBarInstance = Object.Instantiate(GlobalConstants.boxFill, anchor);
        fillBarInstance.transform.localPosition = new Vector3(0f, 2f, 0f);

        fillBarSpriteRenderer = fillBarInstance.GetComponent<SpriteRenderer>();
        fillBarSpriteRenderer.material = new Material(fillBarSpriteRenderer.material);
        fillBarSpriteRenderer.material.SetColor("_colorEmpty", GlobalConstants.badColor);
        fillBarSpriteRenderer.material.SetColor("_colorFilled", GlobalConstants.goodColor);

        keyZ = new GameObject("KeyZ");
        keyZ.transform.parent = fillBarInstance.transform;
        keyZ.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        keyZ.transform.localScale = new Vector3(1f, 1f, 1f); // same as parent
        keyZAnimator = keyZ.AddComponent<UIKeyAnimator>();
        keyZAnimator.Init(GlobalConstants.keyZ, KeyState.Up);
    }
    protected override void DestroyUI()
    {
        if (fillBarInstance == null) return;

        Object.Destroy(fillBarInstance);
        fillBarInstance = null;
        fillBarSpriteRenderer = null;
    }

    protected override void StartQTE(Interactable parent)
    {
        CreateUI(parent.transform);
        StartMashing();
        zReleased = true;
    }
    public override void EndQTE(Interactable parent)
    {
        parent.ResetInteracts();
        ResetMashing();
        DestroyUI();
    }
}