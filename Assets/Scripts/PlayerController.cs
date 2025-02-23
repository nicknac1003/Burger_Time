using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    // Player Input
    private PlayerInput playerInput;
    private InputAction pauseAction;
    private InputAction ticketAction;
    private InputAction moveAction;
    private InputAction zAction;
    private InputAction xAction;
    private InputAction cAction;

    [Header("Physics Paramters")]
    [SerializeField] private float moveSpeed; // m/s
    [SerializeField] private float playerRadius;
    [SerializeField] private float velocityDecay; // m/s^2
    private float decayFactor;

    [Header("Gameplay Variables")]
    public Transform holdAnchor;
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;

    private List<Interactable> interactables = new();
    private Interactable closestInteractable;
    private Holdable holding;

    public bool lockedInPlace = false;
    public Vector2 wishDirection { get; private set; }
    public int direction { get; private set; } = 0; // 0 = down, 1 = left, 2 = right, 3 = up
    private Vector2 lockedLastDirection; // prevent unwanted movement when leaving locked state
    private bool goodUnlock = true;   // prevent unwanted movement when leaving locked state
    private Vector2 prevWishDirection;
    public static bool LockedInPlace() => Instance.lockedInPlace;

    public static void SetHolding(Holdable holdable) => Instance.holding = holdable;

    public static void LockPlayer()
    {
        if (Instance.lockedInPlace) return;
        Instance.lockedInPlace = true;
        Instance.wishDirection = Vector2.zero;
    }
    public static void UnlockPlayer()
    {
        Debug.Log("Unlocking player");
        if (Instance.lockedInPlace == false) return;
        Instance.lockedInPlace = false;
        Instance.lockedLastDirection = Instance.wishDirection;
        Instance.goodUnlock = false;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        moveAction = playerInput.actions.FindAction("Move");

        pauseAction = playerInput.actions.FindAction("Pause");
        pauseAction.started += _ => GameManager.Instance.HandlePauseGame();

        zAction = playerInput.actions.FindAction("Z");
        zAction.started += _ => { if (!GameManager.GamePaused() && closestInteractable != null) closestInteractable.InteractZ(true); };
        zAction.canceled += _ => { if (!GameManager.GamePaused() && closestInteractable != null) closestInteractable.InteractZ(false); };

        xAction = playerInput.actions.FindAction("X");
        xAction.started += _ => { if (!GameManager.GamePaused() && closestInteractable != null) closestInteractable.InteractX(true); };
        xAction.canceled += _ => { if (!GameManager.GamePaused() && closestInteractable != null) closestInteractable.InteractX(false); };

        cAction = playerInput.actions.FindAction("C");
        cAction.started += _ => { if (!GameManager.GamePaused() && closestInteractable != null) closestInteractable.InteractC(true); };
        cAction.canceled += _ => { if (!GameManager.GamePaused() && closestInteractable != null) closestInteractable.InteractC(false); };

        decayFactor = 1 - velocityDecay * Time.fixedDeltaTime;
    }
    void Start()
    {
        holdAnchor = transform.Find("HoldingPos").transform;
    }
    void Update()
    {
        if (GameManager.GamePaused()) return;

        if (!lockedInPlace) wishDirection = moveAction.ReadValue<Vector2>().normalized;

        if (goodUnlock == false)
        {
            goodUnlock = UniqueDirection(lockedLastDirection, wishDirection);
        }

        if (closestInteractable != null)
        {
            closestInteractable.InteractMove(wishDirection);
        }

        UpdateInteract();
        if (holding != null && Input.GetKeyDown(KeyCode.C))
        {
            DestroyItem();
        }
    }

    void FixedUpdate()
    {
        if (GameManager.GamePaused()) return;

        if (lockedInPlace == false && goodUnlock)
        {
            UpdatePosition();
        }
        UpdateAnimation();
    }
    private void UpdateAnimation()
    {
        if (wishDirection.magnitude > 0.1f)
        {
            if (animator.GetBool("Moving") == false)
            {
                animator.SetTrigger("DirectionChange");
            }

            animator.SetBool("Moving", true);

            if (prevWishDirection.y <= -0.1 && wishDirection.y > -0.1)
            {
                animator.SetTrigger("DirectionChange");
            }
            else if (prevWishDirection.y >= 0.1 && wishDirection.y < 0.1)
            {
                animator.SetTrigger("DirectionChange");
            }
            else if (prevWishDirection.x <= -0.1 && wishDirection.x > -0.1)
            {
                animator.SetTrigger("DirectionChange");
            }
            else if (prevWishDirection.x >= 0.1 && wishDirection.x < 0.1)
            {
                animator.SetTrigger("DirectionChange");
            }

            animator.SetFloat("xVel", wishDirection.x);
            animator.SetFloat("yVel", wishDirection.y);

            prevWishDirection = wishDirection;

        }
        else
        {
            animator.SetBool("Moving", false);
        }
    }
    private void UpdatePosition()
    {
        rb.MovePosition(rb.position + wishDirection * moveSpeed * Time.fixedDeltaTime);
    }

    public static bool UniqueDirection(Vector2 a, Vector2 b)
    {
        // Are the vectors in a different enough direction?
        bool uniqueDirection = Vector2.Angle(a, b) > 30f;

        // Do the vectors differ enough in magnitude?
        bool uniqueMagnitude = Mathf.Abs(a.magnitude - b.magnitude) > 0.25f;

        return uniqueDirection || uniqueMagnitude;
    }

    private void AddInteractable(Interactable interactable)
    {
        if (interactable == null) return;
        interactables.Add(interactable);
    }
    private void RemoveInteractable(Interactable interactable)
    {
        if (interactable == null) return;
        interactables.Remove(interactable);
        interactable.ResetInteracts();
    }

    private Interactable GetClosestInteractable()
    {
        if (interactables.Count <= 0) return null;

        Interactable closest = interactables[0];
        Vector3 closestToPlayer = Vector3.Scale(closest.transform.position - transform.position, new Vector3(1, 1, 0)); // ignore Y axis

        for (int i = 1; i < interactables.Count; i++)
        {
            Vector3 toPlayer = Vector3.Scale(interactables[i].transform.position - transform.position, new Vector3(1, 1, 0)); // ignore Y axis

            if (toPlayer.magnitude < closestToPlayer.magnitude)
            {
                closest = interactables[i];
                closestToPlayer = toPlayer;
            }
        }

        return closest;
    }

    private void UpdateInteract()
    {
        closestInteractable = GetClosestInteractable();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {

            AddInteractable(other.GetComponent<Interactable>());
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            RemoveInteractable(other.GetComponent<Interactable>());
        }
    }

    public static bool HoldingItem()
    {
        return Instance.holding != null;
    }
    public static Holdable GetItem()
    {
        return Instance.holding;
    }
    public static bool GrabItem(Holdable item)
    {
        if (HoldingItem()) return false;

        Instance.holding = item;
        Instance.holding.transform.SetParent(Instance.holdAnchor);
        Instance.holding.transform.localPosition = Vector3.zero;
        if (Instance.closestInteractable is IngredientCrate ic) Instance.animator.SetTrigger("Pickup");

        return true;
    }
    public static bool DestroyItem()
    {
        if (HoldingItem() == false) return false;

        Destroy(Instance.holding.gameObject);
        Instance.holding = null;

        return true;
    }

    public void SetPlayerDirection(int dir)
    {
        direction = dir;
        animator.SetInteger("Direction", dir);
    }

    public static Direction GetDirection(Vector2 moveInput, float squaredThreshold = 0.25f)
    {
        if (moveInput.sqrMagnitude <= squaredThreshold) return Direction.None;
        if (Vector2.Angle(Vector2.up, moveInput) <= 45) return Direction.Up;
        if (Vector2.Angle(Vector2.right, moveInput) <= 45) return Direction.Right;
        if (Vector2.Angle(Vector2.down, moveInput) <= 45) return Direction.Down;
        return Direction.Left;
    }

}
