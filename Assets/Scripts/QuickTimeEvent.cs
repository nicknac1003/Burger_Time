using UnityEngine;

public interface class QuickTimeEvent : MonoBehaviour
{
    public Breakable breakable;

    public abstract void InteractZ(bool held);
}
