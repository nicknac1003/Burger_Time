using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    InputAction pauseAction;
    InputAction moveAction;
    InputAction zAction;
    InputAction xAction;
    InputAction cAction;

    [Header("Walk Paramters")]
    public  float moveSpeed; // m/s

    [Header("Physics Parameters")]
    public  float playerHeight;
    public  float playerRadius;
    private float skinnyRadius;

    public  float skinWidth; // tolerance to prevent bounding box from intersecting with walls

    public  float velocityDecay; // m/s^2
    private float decayFactor;

    public  Vector3 acceleration; // m/s^2
    public  Vector3 velocity;     // m/s
    public  float   velocityMagnitude;
    public  float   accelerationMagnitude;

    private List<Interactable> interactables;

    void Awake()
    {
        moveAction   = playerInput.actions.FindAction("Move");

        skinnyRadius = playerRadius - skinWidth;
        decayFactor  = 1 - velocityDecay * Time.fixedDeltaTime;
    }
    void Start()
    {
        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        UpdateInteract();   
    }

    void FixedUpdate()
    {
        UpdatePosition();
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
        Vector2 input = moveAction.ReadValue<Vector2>().normalized; // wish direction

        float magnitude = moveSpeed * velocityDecay / decayFactor;

        Vector3 horizontalAcceleration = Vector3.forward * input.y * magnitude + Vector3.right * input.x * magnitude; // m/s^2

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
        if (Physics.CapsuleCast(origin + Vector3.up * playerRadius, origin + Vector3.up * (playerHeight - playerRadius), skinnyRadius, displacement.normalized, out RaycastHit hit, displacement.magnitude + skinWidth, LayerMask.GetMask("Wall")) == false)
        {
            Debug.DrawLine(origin, origin + displacement, Color.blue, Time.fixedDeltaTime);
            return displacement;
        }

        // Find new origin point (where the collision occurred)
        Vector3 reducedDisplacement = displacement.normalized * (hit.distance - skinWidth);

        // Calculate leftover displacement after collision
        Vector3 leftoverDisplacement = displacement - reducedDisplacement;

        // Ensure there is enough room for collision check to work, otherwise set reducedDisplacement to zero
        if (reducedDisplacement.magnitude <= skinWidth)
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

    public void AddInteractable(Interactable interactable)
    {
        interactables.Add(interactable);
    }
    public void RemoveInteractable(Interactable interactable)
    {
        interactables.Remove(interactable);
    }

    private Interactable GetClosestInteractable()
    {
        Interactable closest = interactables[0];
        Vector3 closestToPlayer = Vector3.Scale(closest.transform.position - transform.position, new Vector3(1, 0, 1)); // ignore Y axis

        for(int i = 1; i < interactables.Count; i++)
        {
            Vector3 toPlayer = Vector3.Scale(interactables[i].transform.position - transform.position, new Vector3(1, 0, 1)); // ignore Y axis

            if(toPlayer.magnitude < closestToPlayer.magnitude)
            {
                closest = interactables[i];
                closestToPlayer = toPlayer;
            }
        }

        return closest;
    }

    private void UpdateInteract()
    {
        if(interactables.Count <= 0) return;

        Interactable closest = GetClosestInteractable();

        if(zAction.ReadValue<float>() > 0f)
        {
            closest.InteractZ();
        }
        else if(xAction.ReadValue<float>() > 0f)
        {
            closest.InteractX();
        }
        else if(cAction.ReadValue<float>() > 0f)
        {
            closest.InteractC();
        }
    }
}
