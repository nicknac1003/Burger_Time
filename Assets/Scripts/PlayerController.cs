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

    public  OrderManager ticketManager;

    private List<Interactable> interactables = new();
    private Interactable closestInteractable;
    private Holdable holding;

    private bool lockedInPlace = false;
    private Vector2 wishDirection;
    private Vector2 lockedLastDirection; // prevent unwanted movement when leaving locked state
    private bool    goodUnlock = true;   // prevent unwanted movement when leaving locked state

    public bool LockedInPlace() => lockedInPlace;
    public void LockPlayer()
    {
        if(lockedInPlace) return;

        lockedInPlace = true;
        wishDirection = Vector2.zero;
        velocity      = Vector3.zero;
    }
    public void UnlockPlayer()
    {
        if(lockedInPlace == false) return;

        lockedInPlace       = false;
        lockedLastDirection = wishDirection;
        goodUnlock          = false;
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

        ticketAction = playerInput.actions.FindAction("Ticket");
        ticketAction.started  += _ => { if(!GameManager.Instance.GamePaused()) ticketManager.Open(); };
        ticketAction.canceled += _ => { if(!GameManager.Instance.GamePaused()) ticketManager.Close(); };

        zAction = playerInput.actions.FindAction("Z");
        zAction.started  += _ => { if (!GameManager.Instance.GamePaused() && closestInteractable != null) closestInteractable.InteractZ(true); };
        zAction.canceled += _ => { if (!GameManager.Instance.GamePaused() && closestInteractable != null) closestInteractable.InteractZ(false); };

        xAction = playerInput.actions.FindAction("X");
        xAction.started  += _ => { if (!GameManager.Instance.GamePaused() && closestInteractable != null) closestInteractable.InteractX(true); };
        xAction.canceled += _ => { if (!GameManager.Instance.GamePaused() && closestInteractable != null) closestInteractable.InteractX(false); };

        cAction = playerInput.actions.FindAction("C");
        cAction.started  += _ => { if (!GameManager.Instance.GamePaused() && closestInteractable != null) closestInteractable.InteractC(true); };
        cAction.canceled += _ => { if (!GameManager.Instance.GamePaused() && closestInteractable != null) closestInteractable.InteractC(false); };

        decayFactor = 1 - velocityDecay * Time.fixedDeltaTime;
    }
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if(GameManager.Instance.GamePaused()) return;

        wishDirection = moveAction.ReadValue<Vector2>().normalized;

        if(goodUnlock == false)
        {
            goodUnlock = UniqueDirection(lockedLastDirection, wishDirection);
        }

        if(closestInteractable != null)
        {
            closestInteractable.InteractMove(wishDirection);
        }

        UpdateInteract();
    }

    void FixedUpdate()
    {
        if(GameManager.Instance.GamePaused()) return;
        
        if(lockedInPlace == false && goodUnlock)
        {
            UpdatePosition();
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

        transform.position = new Vector3(transform.position.x, 0, transform.transform.position.z); // ALWAYS on floor level (Y = 0)
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

    private bool UniqueDirection(Vector3 a, Vector3 b)
    {
        // Are the vectors in a different enough direction?
        bool uniqueDirection = Vector3.Angle(a, b) > 30f;

        // Do the vectors differ enough in magnitude?
        bool uniqueMagnitude = Mathf.Abs(a.magnitude - b.magnitude) > 0.25f;

        return uniqueDirection || uniqueMagnitude;
    }

    public void AddInteractable(Interactable interactable)
    {
        interactables.Add(interactable);
    }
    public void RemoveInteractable(Interactable interactable)
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

    public bool HoldingItem()
    {
        return holding != null;
    }
    public Holdable GetItem()
    {
        return holding;
    }
    public bool GrabItem(Holdable item)
    {
        if(HoldingItem()) return false;

        holding = item;
        holding.transform.SetParent(holdAnchor);
        holding.transform.localPosition = Vector3.zero;
        
        return true;
    }
    public bool DestroyItem()
    {
        if(HoldingItem() == false) return false;

        Destroy(holding.gameObject);
        holding = null;

        return true;
    }
}
