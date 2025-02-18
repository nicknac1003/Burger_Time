using UnityEngine;

[System.Serializable]
public class HoldQTE : QuickTimeEvent
{
    [SerializeField] private GameObject clock;

    private GameObject clockInstance;
    private SpriteRenderer clockSpriteRenderer;

    [Tooltip("How long it takes to complete QTE.")]
    [Range(0.5f, 8f)][SerializeField] private float time  = 4f;

    [Tooltip("How fast progress depletes when not holding.")]
    [Range(0f, 2f)][SerializeField] private float drain = 0.5f; // Speed progress depletes when not holding

    private float progress;

    private float fillSpeed  = 1f; // Multiplier for how fast progress fills
    private float drainSpeed = 1f; // Multiplier for how fast progress depletes

    public HoldQTE() // default constructor
    {
        time     = 4f;
        drain    = 0.5f;
        progress = -1f;
    }

    public HoldQTE(float holdTime, float drainRate)
    {
        time = holdTime;
        drain = drainRate;
        progress = -1f;
    }

    public void SetFillSpeed(float speed)  => fillSpeed  = speed;
    public void ResetFillSpeed()           => fillSpeed  = 1f;
    public void SetDrainSpeed(float speed) => drainSpeed = speed;
    public void ResetDrainSpeed()          => drainSpeed = 1f;

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        if(progress < 0f) // Initialize
        {
            progress = 0f;
            CreateUI(parent.transform);
        }

        if (zPressed == false)
        {
            if (progress > 0f)
            {
                progress = Mathf.Max(progress - Time.deltaTime * drain * drainSpeed, 0f); // deplete progress by repairDrain per second

                clockSpriteRenderer.material.SetFloat("_progress", progress / time); // normalize progress to 0-1 range
            }
            return 0f;
        }

        if (progress >= time)
        {
            progress = -1f;
            parent.ResetInteracts();
            DestroyUI();
            return 1f;
        }

        progress += Time.deltaTime * fillSpeed;

        clockSpriteRenderer.material.SetFloat("_progress", progress / time); // normalize progress to 0-1 range

        return 0f;
    }

    public override bool InProgress()
    {
        return progress < time && progress >= 0f;
    }

    public override void CreateUI(Transform parent)
    {
        clockInstance = Object.Instantiate(clock, parent);
        clockInstance.transform.localPosition = new Vector3(0f, 2f, 0f);

        clockSpriteRenderer = clockInstance.GetComponent<SpriteRenderer>();
        clockSpriteRenderer.material = new Material(clockSpriteRenderer.material);
        clockSpriteRenderer.material.SetColor("_colorEmpty", GlobalConstants.badColor);
        clockSpriteRenderer.material.SetColor("_colorFilled", GlobalConstants.goodColor);
    }

    public override void DestroyUI()
    {
        Object.Destroy(clockInstance);
        clockSpriteRenderer = null;
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }
}
