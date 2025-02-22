using Unity.VisualScripting;
using UnityEngine;

public class Stove : Appliance
{
    [SerializeField] private float BrokenCookTimeMultiplier = 4f;
    [SerializeField] private float cookTime = 10f;
    [SerializeField] private float BurntTime = 20f;
    [Header("Cooking Audio Parameters")]
    [SerializeField] private AudioClip cookingClip;
    [SerializeField] private AudioSource audioSource;

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

        IngredientObject patty = holdable as IngredientObject;
        if (patty.Type() != IngredientType.Patty) return;

        //increase cooktimer. faster if broken
        float cookTimeAdd = working ? Time.deltaTime : Time.deltaTime * BrokenCookTimeMultiplier;
        if (patty != null && holdable != null)
        {
            PlayClip(false);
            patty.UpdateCookTime(cookTimeAdd);
            if (patty.GetCookTime() >= BurntTime)
            {
                patty.ChangeState(IngredientState.Burnt);
                PlayClip(true);
            }
            else if (patty.GetCookTime() >= cookTime)
            {
                patty.ChangeState(IngredientState.Cooked);
                PlayClip(true);
            }
        }
    }

    public void PlayClip(bool force)
    {
        if (force)
        {
            audioSource.Stop();
            audioSource.Play();
        }
        else
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = cookingClip;
                audioSource.Play();
            }
        }
    }


}