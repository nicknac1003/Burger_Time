using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class Breakable
{
    [Tooltip("Can this item break?")]
    [SerializeField] private bool canBreak = true;

    [Tooltip("Can this item break while being used?")]
    [SerializeField] private bool canBreakDuringUse = false;

    [Tooltip("How often we check if something breaks in seconds.")]
    [Range(0.5f, 5f)][SerializeField] private float interval = 1f;

    [Tooltip("How long after breaking before we can break again.")]
    [Range(1f, 60f)][SerializeField] private float safetyTime = 5f;

    [Tooltip("Chance of breaking per interval check as a decimal percentage.")]
    [Range(0.008f, 0.05f)][SerializeField] private float breakChance = 0.05f;

    [Tooltip("Quicktime Event to repair the item.")]
    [SerializeReference] private QuickTimeEvent repairQTE = new HoldQTE();

    [Tooltip("What item needs to be held to perform the repair?")]
    [SerializeField] private Holdable requiredHoldable;

    [Tooltip("FX to play while broken.")]
    [SerializeField] private Transform vfxBreakPrefab;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private float breakVolume = 0.2f;
    [SerializeField] private AudioSource breakAudioSource;

    private Transform vfxInScene;

    [SerializeField] private bool requireHoldable = false;

    [Tooltip("Anchor to place VFX.")]
    [SerializeField] private Transform vfxAnchor;

    private float breakTimer = 0f;
    private bool broken = false;

    public bool IsBroken() => broken;
    public bool CanBreak() => canBreak;

    public void HandleBreaking(Appliance parent)
    {
        if (broken) return;

        if (canBreakDuringUse == false && parent.InUse()) return;

        if (breakTimer >= interval)
        {
            breakTimer = 0f;

            if (Random.value < breakChance)
            {
                Break(parent);
            }
        }

        breakTimer += Time.deltaTime;
    }

    public void HandleRepairing(bool pressed, Appliance parent)
    {
        if (broken == false) return;

        if (repairQTE == null)
        {
            Debug.LogError("No repair QTE assigned to " + this);
            return;
        }

        if ((requireHoldable && requiredHoldable != null && PlayerController.GetItem() == requiredHoldable) || !requireHoldable)
        {
            Debug.Log("QTE");
            if (repairQTE.QTE(pressed, false, false, Vector2.zero, parent) > 0f)
                if ((requireHoldable && requiredHoldable != null && PlayerController.GetItem() == requiredHoldable) || !requireHoldable)
                {
                    if (repairQTE.QTE(pressed, false, false, Vector2.zero, parent) > 0f)
                    {
                        Repair(parent);
                    }
                }
        }

    public void Break(Appliance parent)
    {
        broken = true;
        Createvfx();
        ToggleBreakSound(true);
        Debug.Log(parent.name + " broke!");
    }

    private void Repair(Appliance parent)
    {
        broken = false;
        breakTimer = -safetyTime; // set to negative safety time so we can reset to 0 for interval checks
        Destroyvfx();
        ToggleBreakSound(false);
        Debug.Log(parent.name + " repaired!");
    }
    private void Createvfx()
    {
        if (vfxBreakPrefab == null) return;
        if (vfxInScene != null) return;

        vfxInScene = Object.Instantiate(vfxBreakPrefab, vfxAnchor.position, Quaternion.identity);
        vfxInScene.transform.SetParent(vfxAnchor);
    }

    private void Destroyvfx()
    {
        if (vfxInScene == null) return;

        Object.Destroy(vfxInScene.gameObject);
        vfxInScene = null;
    }

    public void ToggleBreakSound(bool on)
    {
        if (breakAudioSource == null || breakSound == null) return;
        breakAudioSource.clip = breakSound;
        if (on)
        {
            breakAudioSource.volume = breakVolume;
            breakAudioSource.Play();
        }
        else
        {
            breakAudioSource.Stop();
        }
    }

    public void SetRequiredHoldable(Holdable holdable) => requiredHoldable = holdable;

}
