using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    private Collider bounds;

    public virtual void InteractZ() { return; }
    public virtual void InteractX() { return; }
    public virtual void InteractC() { return; }

    private void Awake()
    {
        bounds = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().AddInteractable(this);
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().RemoveInteractable(this);
        }
    }
}