using UnityEngine;

[System.Serializable]
public class HoldQTE : QuickTimeEvent
{
    private GameObject     circle;
    private SpriteRenderer circleSpriteRenderer;

    [Tooltip("How long it takes to complete QTE.")]
    [Range(0.5f, 8f)][SerializeField] private float time  = 4f;

    [Tooltip("How fast progress depletes when not holding.")]
    [Range(0f, 2f)][SerializeField] private float drain = 0.5f; // Speed progress depletes when not holding

    private float progress;

    private float fillSpeed  = 1f; // Multiplier for how fast progress fills
    private float drainSpeed = 1f; // Multiplier for how fast progress depletes

    public HoldQTE() : base(false) // default constructor
    {
        time     = 4f;
        drain    = 0.5f;
        progress = -1f;
    }
    public HoldQTE(float holdTime, float drainRate) : base(false)
    {
        time = holdTime;
        drain = drainRate;
        progress = -1f;
    }

    public void SetFillSpeed(float speed)  => fillSpeed  = speed;
    public void ResetFillSpeed()           => fillSpeed  = 1f;
    public void SetDrainSpeed(float speed) => drainSpeed = speed;
    public void ResetDrainSpeed()          => drainSpeed = 1f;

    private void ResetProgress()           => progress   = -1f;
    private void StartProgress()           => progress   = 0f;

    public override bool InProgress()
    {
        return progress < time && progress >= 0f;
    }

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        if(zPressed == false)
        {
            if(progress > 0f)
            {
                progress = Mathf.Max(progress - Time.deltaTime * drain * drainSpeed, 0f); // deplete progress by repairDrain per second

                circleSpriteRenderer.material.SetFloat("_progress", progress / time); // normalize progress to 0-1 range
            }
            else
            {
                EndQTE(parent);
            }
            return 0f;
        }

        if(progress < 0f)
        {
            StartQTE(parent);
        }

        if (InProgress() == false)
        {
            EndQTE(parent);
            return 1f;
        }

        progress += Time.deltaTime * fillSpeed;

        circleSpriteRenderer.material.SetFloat("_progress", progress / time); // normalize progress to 0-1 range

        return 0f;
    }

    protected override void CreateUI(Transform anchor)
    {
        DestroyUI();

        circle = Object.Instantiate(GlobalConstants.circleFill, anchor);
        circle.transform.localPosition = new Vector3(0f, 2f, 0f);

        circleSpriteRenderer = circle.GetComponent<SpriteRenderer>();
        circleSpriteRenderer.material = new Material(circleSpriteRenderer.material);
        circleSpriteRenderer.material.SetColor("_colorEmpty", GlobalConstants.badColor);
        circleSpriteRenderer.material.SetColor("_colorFilled", GlobalConstants.goodColor);
    }
    protected override void DestroyUI()
    {
        if(circle == null) return;

        Object.Destroy(circle);
        circle       = null;
        circleSpriteRenderer = null;
    }

    protected override void StartQTE(Interactable parent)
    {
        // Do not reset interacts - hold allowed to pass through
        StartProgress();
        CreateUI(parent.transform);
    }
    public override void EndQTE(Interactable parent)
    {
        parent.ResetInteracts();
        ResetProgress();
        DestroyUI();
    }
}
