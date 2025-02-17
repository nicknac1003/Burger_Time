using UnityEngine;

public class HoldQTE : QuickTimeEvents
{
    public void Update()
    {
        HandleRepairing();
    }

    public override void InteractZ(bool held)
    {
        repairing = held;
    }
    public void HandleRepairing()
    {
        if (broken == false) return;

        if (repairing == false)
        {
            if (repairTimer > 0f)
            {
                repairTimer = Mathf.Max(repairTimer - Time.deltaTime * repairDrain, 0f); // deplete progress by repairDrain per second

                Debug.Log("Stopped " + gameObject.name + " | Progress: " + repairTimer + " / " + repairTime);
            }
            return;
        }

        Debug.Log("Repairing " + gameObject.name + " | Progress: " + repairTimer + " / " + repairTime);

        if (repairTimer >= repairTime)
        {
            Repair();
            return;
        }

        repairTimer += Time.deltaTime;
    }


}
