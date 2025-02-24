using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

[System.Serializable]
public class Breakable
{
    [Tooltip("Can this item break?")]
    [SerializeField] private bool canBreak = true;

    [Tooltip("Can this item break while being used?")]
    [SerializeField] private bool canBreakDuringUse = false;

    [Tooltip("How often we check if something breaks in seconds.")]
    [Range(1f, 30f)][SerializeField] private float interval = 1f;

    [Tooltip("How long after breaking before we can break again.")]
    [Range(1f, 60f)][SerializeField] private float safetyTime = 5f;

    [Tooltip("Chance of breaking per interval check as a decimal percentage.")]
    [Range(0.001f, 0.01f)][SerializeField] private float breakChance = 0.008f;

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

        if ((requireHoldable && requiredHoldable != null && PlayerController.GetItem() != null && PlayerController.GetItem().GetType() == requiredHoldable.GetType()) || !requireHoldable)
        {
            float QTEscore = repairQTE.QTE(pressed, false, false, Vector2.zero, parent);
            if (QTEscore > 0f)  {
                Repair(parent);
            }

        }
    }
    public void Break(Appliance parent)
    {
        broken = true;
        Createvfx();
        ToggleBreakSound(true);
    }

    private void Repair(Appliance parent)
    {
        broken = false;
        breakTimer = -safetyTime; // set to negative safety time so we can reset to 0 for interval checks
        Destroyvfx();
        ToggleBreakSound(false);
    }
    private void Createvfx()
    {
        if (vfxBreakPrefab == null) return;
        if (vfxInScene != null) return;

        // Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
        vfxInScene = Object.Instantiate(vfxBreakPrefab, vfxAnchor.position, Quaternion.identity);
        vfxInScene.transform.SetParent(vfxAnchor);
        // vfxInScene.GetComponent<ParticleSystem>().Play();
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
}
