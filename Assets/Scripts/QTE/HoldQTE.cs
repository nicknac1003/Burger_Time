using UnityEngine;

[System.Serializable]
public class HoldQTE : QuickTimeEvent
{
    private GameObject circle;
    private SpriteRenderer circleSpriteRenderer;

    [Tooltip("How long it takes to complete QTE.")]
    [Range(0.5f, 8f)][SerializeField] private float time = 4f;

    [Tooltip("How fast progress depletes when not holding.")]
    [Range(0f, 2f)][SerializeField] private float drain = 0.5f; // Speed progress depletes when not holding

    private GameObject keyZ;
    private UIKeyAnimator keyZAnimator;

    private float progress;

    private float fillSpeed = 1f; // Multiplier for how fast progress fills
    private float drainSpeed = 1f; // Multiplier for how fast progress depletes

    // bools to play a sound at each quarter of the progress
    private bool played01 = false;
    private bool played25 = false;
    private bool played50 = false;
    private bool played75 = false;
    private bool played100 = false;

    public HoldQTE() : base(false) // default constructor
    {
        time = 4f;
        drain = 0.5f;
        progress = -1f;
    }
    public HoldQTE(float holdTime, float drainRate) : base(false)
    {
        time = holdTime;
        drain = drainRate;
        progress = -1f;
    }

    public void SetFillSpeed(float speed) => fillSpeed = speed;
    public void ResetFillSpeed() => fillSpeed = 1f;
    public void SetDrainSpeed(float speed) => drainSpeed = speed;
    public void ResetDrainSpeed() => drainSpeed = 1f;

    private void ResetProgress() => progress = -1f;
    private void StartProgress() => progress = 0f;

    public override bool InProgress()
    {
        return progress < time && progress >= 0f;
    }

    protected override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        if (zPressed == false)
        {
            if (progress > 0f)
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

        if (progress < 0f)
        {
            StartQTE(parent);
        }

        if (InProgress() == false)
        {
            EndQTE(parent);
            return 1f;
        }

        keyZAnimator.PushKey(0.15f);

        progress += Time.deltaTime * fillSpeed;
        checkSoundProgress();

        circleSpriteRenderer.material.SetFloat("_progress", progress / time); // normalize progress to 0-1 range

        return 0f;
    }

    private void checkSoundProgress() // mostly for wrench cranking sound
    {
        if (progress / time > 0.01 && !played25)
        {
            PlayProgressSound();
            played01 = true;
        }
        if (progress / time > 0.25 && !played25)
        {
            PlayProgressSound();
            played25 = true;
        }
        if (progress / time > 0.5 && !played50)
        {
            PlayProgressSound();
            played50 = true;
        }
        if (progress / time > 0.75 && !played75)
        {
            PlayProgressSound();
            played75 = true;
        }
        if (progress / time > 0.90 && !played100)
        {
            PlayProgressSound();
            played100 = true;
        }
    }

    protected override void CreateUI(Transform anchor)
    {
        DestroyUI();

        circle = Object.Instantiate(GlobalConstants.circleFill, anchor);
        circle.transform.localPosition = new Vector3(0f, 2f, 0f);

        circleSpriteRenderer = circle.GetComponent<SpriteRenderer>();
        circleSpriteRenderer.sortingOrder = 10;
        circleSpriteRenderer.material = new Material(circleSpriteRenderer.material);
        circleSpriteRenderer.material.SetColor("_colorEmpty", GlobalConstants.badColor);
        circleSpriteRenderer.material.SetColor("_colorFilled", GlobalConstants.goodColor);

        keyZ = new GameObject("KeyZ");
        keyZ.transform.parent = circle.transform;
        keyZ.transform.localPosition = new Vector3(0f, 1f, 0f);
        keyZAnimator = keyZ.AddComponent<UIKeyAnimator>();
        keyZAnimator.Init(GlobalConstants.keyZ, KeyState.Up);
    }
    protected override void DestroyUI()
    {
        if (circle == null) return;

        Object.Destroy(circle);
        circle = null;
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
        isActive = false;
        ResetProgress();
        EndProgressSound();
        DestroyUI();
    }
}
