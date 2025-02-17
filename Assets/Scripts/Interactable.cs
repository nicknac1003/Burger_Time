using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    public virtual void InteractZ(bool held) { return; }
    public virtual void InteractX(bool held) { return; }
    public virtual void InteractC(bool held) { return; }

    public void ResetInteracts()
    {
        InteractZ(false);
        InteractX(false);
        InteractC(false);
    }
}