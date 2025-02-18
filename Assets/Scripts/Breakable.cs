using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class Breakable
{
    [Tooltip("Can this item break?")]
    [SerializeField] private bool canBreak = true;

    [Tooltip("How often we check if something breaks in seconds.")]
    [Range(0.5f, 5f)][SerializeField] private float interval = 1f;
    
    [Tooltip("How long after breaking before we can break again.")]
    [Range(0f, 20f)][SerializeField] private float safetyTime = 5f;

    [Tooltip("Chance of breaking per interval check as a decimal percentage.")]
    [Range(0.008f, 0.05f)][SerializeField] private float breakChance = 0.05f;

    [Tooltip("Quicktime Event to repair the item.")]
    [SerializeReference] private QuickTimeEvent repairQTE = new HoldQTE();

    [Tooltip("What item needs to be held to perform the repair?")]
    [SerializeField] private Holdable holdable;

    [Tooltip("VFX to play while broken.")]
    [SerializeField] private VisualEffect vfxBreak;

    [Tooltip("VFX to play when repairing.")]
    [SerializeField] private VisualEffect vfxRepair;

    private float breakTimer = 0f;
    private bool  broken     = false;

    public bool IsBroken() => broken;
    public bool CanBreak() => canBreak;

    public void HandleBreaking()
    {
        if(broken) return;

        if(breakTimer >= interval)
        {
            breakTimer = 0f;

            if(Random.value < breakChance)
            {
                Break();
            }
        }

        breakTimer += Time.deltaTime;
    }

    public void HandleRepairing(bool pressed, Interactable parent)
    {
        if(broken == false) return;

        if(repairQTE == null)
        {
            Debug.LogError("No repair QTE assigned to " + this);
            return;
        }

        if(repairQTE.PerformQTE(pressed, false, false, Vector2.zero, parent) > 0f)
        {
            Repair();
        }
    }

    public void Break()
    {
        broken = true;
        Debug.Log(this + " broke!");
    }

    private void Repair()
    {
        broken      = false;
        breakTimer  = -safetyTime; // set to negative safety time so we can reset to 0 for interval checks

        Debug.Log(this + " repaired!");
    }
}
