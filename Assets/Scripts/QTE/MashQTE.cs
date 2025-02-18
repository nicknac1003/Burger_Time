using UnityEngine;

public class MashQTE : QuickTimeEvent
{
    [Tooltip("The number of times the player must press the button to succeed.")]
    [Range(3, 20)][SerializeField] private int mashCount = 10;

    [SerializeField] private GameObject fillBar;

    private GameObject     fillBarInstance;
    private SpriteRenderer fillBarSpriteRenderer;

    private bool releasedKey  = true;
    private int  mashProgress = 0;

    public MashQTE() : base(true) { mashCount = 10; }
    public MashQTE(int count) : base(true) { mashCount = count; }

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
            if(releasedKey)
            {
                mashProgress++;
            }
        }

        if(InProgress() == false)
        {
            float score = mashProgress >= mashCount ? 1f : 0f;
            EndQTE(parent);
            return score;
        }

        releasedKey = !zPressed;

        float progress = mashProgress / (float)mashCount;

        fillBarSpriteRenderer.material.SetFloat("_progress", progress);

        return 0f;
    }

    protected override void CreateUI(Transform parent)
    {
        DestroyUI();

        fillBarInstance = Object.Instantiate(fillBar, parent);
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
        releasedKey = true;
    }
    public override void EndQTE(Interactable parent)
    {
        parent.ResetInteracts();
        ResetMashing();
        DestroyUI();
    }
}