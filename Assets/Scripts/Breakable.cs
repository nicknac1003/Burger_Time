using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class Breakable : Interactable
{
    [SerializeField] private float interval    = 1f;    // How often we check if something breaks
    [SerializeField] private float safetyTime  = 5f;    // 0 to 20 seconds
    [SerializeField] private float breakChance = 0.05f; // 0.008 to 0.05

    [SerializeField] private float repairTime   = 4f;    // How long it takes to repair
    [SerializeField] private float repairDrain  = 0.5f;  // If stopped repairing, speed progress depletes

    [SerializeField] private VisualEffect vfx;

    private float breakTimer  = 0f;
    private bool  broken      = false;
    private float repairTimer = 0f;
    private bool  repairing   = false;

    public override void InteractZ()
    {
        if(broken)
        {
            Repairing();
        }
    }

    void Update()
    {
        if(repairing == false && repairTimer > 0f)
        {
            repairTimer = Mathf.Max(repairTimer - Time.deltaTime * repairDrain, 0f); // deplete progress by repairDrain per second
        }
        
        if(broken) return;

        if(breakTimer >= interval)
        {
            RollBreak();
        }

        breakTimer += Time.deltaTime;
    }

    private void RollBreak()
    {
        breakTimer = 0f;

        if(Random.value < breakChance)
        {
            Break();
        }
    }

    private void Break()
    {
        broken = true;

        Debug.Log(gameObject.name + " broke!");
    }

    public void Repairing()
    {
        Debug.Log("Repairing " + gameObject.name + " | Progress: " + repairTimer + " / " + repairTime);
        
        repairing = true;

        if(repairTimer >= repairTime)
        {
            Repair();
            return;
        }

        repairTimer += Time.deltaTime;
    }

    private void Repair()
    {
        broken      = false;
        breakTimer  = -safetyTime; // set to negative safety time so we can reset to 0 for interval checks
        repairTimer = 0f;

        Debug.Log(gameObject.name + " repaired!");
    }
}
