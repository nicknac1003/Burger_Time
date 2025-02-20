using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    // Stores the interact state
    protected bool    zPressed  = false;
    private   bool    zReleased = true;
    protected bool    xPressed  = false;
    private   bool    xReleased = true;
    protected bool    cPressed  = false;
    private   bool    cReleased = true;
    protected Vector2 moveInput = Vector2.zero;

    // Set from Player Controller
    public void InteractZ(bool held)
    {
        zPressed = held;
        if(held && zReleased) 
        {
            OnZ();
        }
        zReleased = !held; 
    }
    public void InteractX(bool held)
    { 
        xPressed = held; 
        if(held && xReleased)
        {
            OnX();
        }
        xReleased = !held;
    }
    public void InteractC(bool held)
    {
        cPressed = held;
        if(held)
        {
            OnC();
        }
        cReleased = !held;
    }
    public void InteractMove(Vector2 vec) { moveInput = vec; }

    // Events to hook into for ONLY presses. No holds.
    protected virtual void OnZ() { return; }
    protected virtual void OnX() { return; }
    protected virtual void OnC() { return; }

    // Reset the interact states
    public void ResetInteracts()
    {
        zPressed = false;
        xPressed = false;
        cPressed = false;
        moveInput = Vector2.zero;
    }
}