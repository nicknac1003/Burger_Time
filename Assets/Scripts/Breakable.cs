using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class Breakable
{
    [SerializeField] private float interval    = 1f;    // How often we check if something breaks
    [SerializeField] private float safetyTime  = 5f;    // 0 to 20 seconds
    [SerializeField] private float breakChance = 0.05f; // 0.008 to 0.05

    [SerializeField] private QuickTimeEvent repairQTE;

    [SerializeField] private VisualEffect vfx;

    private float breakTimer = 0f;
    private bool  broken     = false;
    private bool  repairing  = false;

    public bool IsBroken() => broken;
    public void SetRepairing(bool repair) => repairing = repair;

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

    public void HandleRepairing()
    {
        if(broken == false) return;

        if(repairQTE.PerformQTE(repairing, false, false, Vector2.zero) > 0f)
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
