using Unity.VisualScripting;
using UnityEngine;

public class Stove : Appliance
{
    [SerializeField] private float BrokenCookTimeMultiplier = 4f;
    [SerializeField] private float cookTime = 10f;
    [SerializeField] private float BurntTime = 20f;
    [Header("Cooking Audio Parameters")]
    [SerializeField] private AudioClip cookingClip;
    [SerializeField] private float cookingClipVolume = 0.5f;
    [SerializeField] private AudioSource audioSource;

    public void Start()
    {
        if (audioSource != null)
        {
            audioSource.clip = cookingClip;
            audioSource.volume = cookingClipVolume;
        }
    }
    protected override void Update()
    {
        base.Update();

        if (holdable != null)
        {
            Cooking();
        }
    }

    public void Cooking()
    {
        Debug.Log("cooking");
        IngredientObject patty = holdable as IngredientObject;
        if (patty.Type() != IngredientType.Patty) return;


        //increase cooktimer. faster if broken
        float cookTimeAdd = working ? Time.deltaTime : Time.deltaTime * BrokenCookTimeMultiplier;
        if (patty != null && holdable != null)
        {
            PlayClip(false);
            patty.UpdateCookTime(cookTimeAdd);
            if (patty.GetCookTime() >= BurntTime && patty.State() != IngredientState.Burnt)
            {
                patty.ChangeState(IngredientState.Burnt);
                PlayClip(true);
            }
            else if (patty.GetCookTime() >= cookTime && patty.State() != IngredientState.Cooked && patty.State() != IngredientState.Burnt)
            {
                patty.ChangeState(IngredientState.Cooked);
                PlayClip(true);
            }
        }
    }

    protected override bool TakeItem()
    {
        if (!base.TakeItem()) return false;

        audioSource.Stop();

        return true;
    }

    public void PlayClip(bool force)
    {
        if (audioSource == null || cookingClip == null) return;
        Debug.Log("Playing");
        if (force)
        {
            Debug.Log("Forcing");
            audioSource.Stop();
            audioSource.Play();
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

}