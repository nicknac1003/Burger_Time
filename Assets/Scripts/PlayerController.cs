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

    [Header("Physics Debug Data")]
    public Vector3 acceleration; // m/s^2
    public Vector3 velocity;     // m/s
    public float velocityMagnitude;
    public float accelerationMagnitude;

    [Header("Gameplay Variables")]
    [SerializeField] private Transform holdAnchor;
    [SerializeField] private Animator animator;

    private GameObject holdingPosition;
    private List<Interactable> interactables = new();
    private Interactable closestInteractable;
    private Holdable holding;

    private bool lockedInPlace = false;
    private Vector2 wishDirection;
    private Vector2 lockedLastDirection; // prevent unwanted movement when leaving locked state
    private bool goodUnlock = true;   // prevent unwanted movement when leaving locked state
    private Vector2 prevWishDirection;
    public static bool LockedInPlace() => Instance.lockedInPlace;
    public static void LockPlayer()
    {
        if (Instance.lockedInPlace) return;

        Instance.lockedInPlace = true;
        Instance.wishDirection = Vector2.zero;
        Instance.velocity = Vector3.zero;
    }
    public static void UnlockPlayer()
    {
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        holdAnchor = transform.Find("HoldingPos").transform;
    }
    void Update()
    {
        if (GameManager.GamePaused()) return;

        wishDirection = moveAction.ReadValue<Vector2>().normalized;

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
            UpdateAnimation();
        }
    }
    private void UpdateAnimation()
    {
        if (velocityMagnitude > 0.1f)
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
        (velocity, acceleration) = CalculateMovement();

        velocityMagnitude = velocity.magnitude;
        accelerationMagnitude = acceleration.magnitude;

        Vector3 actualDisplacement = CollideAndSlide(transform.position, velocity * Time.fixedDeltaTime);
        actualDisplacement.y = 0;

        _ = CollideAndSlide(transform.position, velocity); // debug draw

        transform.position += actualDisplacement;

        //transform.position = new Vector3(transform.position.x, 0, transform.transform.position.z); // ALWAYS on floor level (Y = 0)
    }

    private (Vector3 v, Vector3 a) CalculateMovement()
    {
        float magnitude = moveSpeed * velocityDecay / decayFactor;

        Vector3 horizontalAcceleration = Vector3.forward * wishDirection.y * magnitude + Vector3.right * wishDirection.x * magnitude; // m/s^2

        Vector3 horizontalVelocity = (new Vector3(velocity.x, 0f, velocity.z) + horizontalAcceleration * Time.fixedDeltaTime) * decayFactor; // m/s

        return (horizontalVelocity, horizontalAcceleration);
    }

    private Vector3 CollideAndSlide(Vector3 origin, Vector3 displacement, int bounceCount = 0)
    {
        if (bounceCount >= 5)
        {
            return Vector3.zero; // prevent infinite recursion - return zero vector
        }

        // Shoot capsule cast and see if displacement will cause collision - return displacement if no collision
        if (Physics.SphereCast(origin + Vector3.up * playerRadius, playerRadius - Physics.defaultContactOffset, displacement.normalized, out RaycastHit hit, displacement.magnitude + Physics.defaultContactOffset, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) == false)
        {
            Debug.DrawLine(origin, origin + displacement, Color.blue, Time.fixedDeltaTime);
            return displacement;
        }

        // Find new origin point (where the collision occurred)
        Vector3 reducedDisplacement = displacement.normalized * (hit.distance - Physics.defaultContactOffset);

        // Calculate leftover displacement after collision
        Vector3 leftoverDisplacement = displacement - reducedDisplacement;

        // Ensure there is enough room for collision check to work, otherwise set reducedDisplacement to zero
        if (reducedDisplacement.magnitude <= Physics.defaultContactOffset)
        {
            reducedDisplacement = Vector3.zero;
        }

        Vector3 projectedDisplacement = Vector3.ProjectOnPlane(leftoverDisplacement, hit.normal);

        Vector3 newOrigin = origin + displacement.normalized * hit.distance;

        Debug.DrawLine(origin + reducedDisplacement, origin + displacement, Color.red, Time.fixedDeltaTime);
        Debug.DrawLine(origin, origin + reducedDisplacement, Color.green, Time.fixedDeltaTime);
        Debug.DrawLine(origin, hit.point, Color.yellow, Time.fixedDeltaTime);

        return reducedDisplacement + CollideAndSlide(newOrigin, projectedDisplacement, bounceCount + 1);
    }

    public static bool UniqueDirection(Vector3 a, Vector3 b)
    {
        // Are the vectors in a different enough direction?
        bool uniqueDirection = Vector3.Angle(a, b) > 30f;

        // Do the vectors differ enough in magnitude?
        bool uniqueMagnitude = Mathf.Abs(a.magnitude - b.magnitude) > 0.25f;

        return uniqueDirection || uniqueMagnitude;
    }

    private void AddInteractable(Interactable interactable)
    {
        interactables.Add(interactable);
    }
    private void RemoveInteractable(Interactable interactable)
    {
        interactables.Remove(interactable);
        interactable.ResetInteracts();
    }

    private Interactable GetClosestInteractable()
    {
        if (interactables.Count <= 0) return null;

        Interactable closest = interactables[0];
        Vector3 closestToPlayer = Vector3.Scale(closest.transform.position - transform.position, new Vector3(1, 0, 1)); // ignore Y axis

        for (int i = 1; i < interactables.Count; i++)
        {
            Vector3 toPlayer = Vector3.Scale(interactables[i].transform.position - transform.position, new Vector3(1, 0, 1)); // ignore Y axis

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            AddInteractable(other.GetComponent<Interactable>());
        }
    }
    private void OnTriggerExit(Collider other)
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

        return true;
    }
    public static bool DestroyItem()
    {
        if (HoldingItem() == false) return false;

        Destroy(Instance.holding.gameObject);
        Instance.holding = null;

        return true;
    }

    // hard coded positions for holding item in hand
    public void holdingPosDown()
    {
        holdAnchor.localPosition = new Vector3(0.37f, -0.654f, -0.05f);
    }
    public void holdingPosUp()
    {
        holdAnchor.localPosition = new Vector3(-0.35f, -0.5f, 0.1f);
    }
    public void holdingPosLeft()
    {
        holdAnchor.localPosition = new Vector3(0.0f, -0.71f, -0.05f);
    }
    public void holdingPosRight()
    {
        holdAnchor.localPosition = new Vector3(0.0f, -0.71f, 0.1f);
    }
}
