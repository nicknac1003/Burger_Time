using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private Collider bounds;

    public virtual void InteractZ() { return; }
    public virtual void InteractX() { return; }
    public virtual void InteractC() { return; }
}