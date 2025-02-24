using UnityEngine;

public enum Direction { Up, Right, Down, Left, None };

public class WindmillQTE : QuickTimeEvent
{
    [Tooltip("The number of times the player has to press Up, Right, Down, Left in order.")]
    [Range(1, 10)][SerializeField] private int revolutions = 3;

    private GameObject fillBarInstance;
    private SpriteRenderer fillBarSpriteRenderer;

    private GameObject keyUp;
    private UIKeyAnimator keyUpAnimator;
    private GameObject keyRight;
    private UIKeyAnimator keyRightAnimator;
    private GameObject keyDown;
    private UIKeyAnimator keyDownAnimator;
    private GameObject keyLeft;
    private UIKeyAnimator keyLeftAnimator;

    private Direction nextDirection = Direction.Up;
    private int mashProgress = -1;
    private int mashCount;

    public WindmillQTE() : base(true) { revolutions = 3; }
    public WindmillQTE(int count) : base(true) { revolutions = count; }

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
        }

        if (cPressed)
        {
            if (mashProgress < 0)
            {
                // Player is taking item out of Appliance before QTE starts
                return 0f;
            }
        }

        if (InProgress() == false)
        {
            float score = mashProgress >= mashCount ? 1f : 0f;
            EndQTE(parent);
            return score;
        }

        Direction direction = PlayerController.GetDirection(moveInput);

        switch (nextDirection)
        {
            case Direction.Up:
                {
                    if (direction == Direction.Up)
                    {
                        nextDirection = Direction.Right;
                        mashProgress++;
                    }
                    else
                    {
                        keyUpAnimator.PushKey(GlobalConstants.alternateAnimationTime);
                        keyRightAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyDownAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyLeftAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                    }
                    break;
                }
            case Direction.Right:
                {
                    if (direction == Direction.Right)
                    {
                        nextDirection = Direction.Down;
                        mashProgress++;
                    }
                    else
                    {
                        keyUpAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyRightAnimator.PushKey(GlobalConstants.alternateAnimationTime);
                        keyDownAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyLeftAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                    }
                    break;
                }
            case Direction.Down:
                {
                    if (direction == Direction.Down)
                    {
                        nextDirection = Direction.Left;
                        mashProgress++;
                    }
                    else
                    {
                        keyUpAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyRightAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyDownAnimator.PushKey(GlobalConstants.alternateAnimationTime);
                        keyLeftAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);

                    }
                    break;
                }
            case Direction.Left:
                {
                    if (direction == Direction.Left)
                    {
                        nextDirection = Direction.Up;
                        mashProgress++;
                    }
                    else
                    {
                        keyUpAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyRightAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyDownAnimator.ReleaseKey(GlobalConstants.alternateAnimationTime);
                        keyLeftAnimator.PushKey(GlobalConstants.alternateAnimationTime);
                    }
                    break;
                }
        }

        float progress = mashProgress / (float)mashCount;

        fillBarSpriteRenderer.material.SetFloat("_progress", progress);

        return 0f;
    }

    protected override void CreateUI(Transform anchor)
    {
        DestroyUI();

        fillBarInstance = Object.Instantiate(GlobalConstants.alternateFill, anchor.position + new Vector3(0f, 2f, 0f), Quaternion.identity);

        fillBarSpriteRenderer = fillBarInstance.GetComponent<SpriteRenderer>();
        fillBarSpriteRenderer.material = new Material(fillBarSpriteRenderer.material);
        fillBarSpriteRenderer.material.SetColor("_colorEmpty", GlobalConstants.badColor);
        fillBarSpriteRenderer.material.SetColor("_colorFilled", GlobalConstants.goodColor);
        fillBarSpriteRenderer.sortingOrder = 10;

        keyUp = new GameObject("KeyUp");
        keyUp.transform.parent = fillBarInstance.transform;
        keyUp.transform.localPosition = new Vector3(0f, 1f, 0f);
        keyUpAnimator = keyUp.AddComponent<UIKeyAnimator>();
        keyUpAnimator.Init(GlobalConstants.keyUp, KeyState.Up);

        keyRight = new GameObject("KeyRight");
        keyRight.transform.parent = fillBarInstance.transform;
        keyRight.transform.localPosition = new Vector3(1f, 0f, 0f);
        keyRightAnimator = keyRight.AddComponent<UIKeyAnimator>();
        keyRightAnimator.Init(GlobalConstants.keyRight, KeyState.Up);

        keyDown = new GameObject("KeyDown");
        keyDown.transform.parent = fillBarInstance.transform;
        keyDown.transform.localPosition = new Vector3(0f, -1f, 0f);
        keyDownAnimator = keyDown.AddComponent<UIKeyAnimator>();
        keyDownAnimator.Init(GlobalConstants.keyDown, KeyState.Up);

        keyLeft = new GameObject("KeyLeft");
        keyLeft.transform.parent = fillBarInstance.transform;
        keyLeft.transform.localPosition = new Vector3(-1f, 0f, 0f);
        keyLeftAnimator = keyLeft.AddComponent<UIKeyAnimator>();
        keyLeftAnimator.Init(GlobalConstants.keyLeft, KeyState.Up);
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
        nextDirection = Direction.Up;
        mashCount = revolutions * 4;
    }
    public override void EndQTE(Interactable parent)
    {
        parent.ResetInteracts();
        isActive = false;
        ResetMashing();
        DestroyUI();
    }
}