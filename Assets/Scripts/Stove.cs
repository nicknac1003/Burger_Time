using Unity.VisualScripting;
using UnityEngine;

public class Stove : Appliance
{
    [SerializeField] private float BrokenCookTimeMultiplier = 4f;
    [SerializeField] private float cookTime = 10f;
    [SerializeField] private float BurntTime = 20f;
    protected override void Update()
    {
        base.Update();

        if (holdable != null)
        {
            Cooking();
        }

    }

    public void Cooking(){
        
        IngredientObject patty = holdable as IngredientObject;
        if (patty.Type() != IngredientType.Patty) return;
        Debug.Log(patty.GetCookTime());
        
        //increase cooktimer. faster if broken
        float cookTimeAdd = working ? Time.deltaTime : Time.deltaTime * BrokenCookTimeMultiplier;
        if (patty != null && holdable != null)
        {
            patty.UpdateCookTime(cookTimeAdd);
            if (patty.GetCookTime() >= BurntTime)
            {
                patty.ChangeState(IngredientState.Burnt);
            }
            else if (patty.GetCookTime() >= cookTime)
            {
                patty.ChangeState(IngredientState.Cooked);
            }
        }
    }
}