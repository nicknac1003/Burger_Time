using UnityEngine;

public class CringeAppliance : Appliance
{
    [SerializeReference] private QuickTimeEvent useApplianceQTE = new MashQTE();
    private bool QTEInProgress = false;

    protected override void Update()
    {
        base.Update();

        if (working)
        {
            if (QTEInProgress)
            {
                if (useApplianceQTE.QTE(zPressed, xPressed, cPressed, moveInput, this) > 0f)
                {
                    OnQTEComplete();
                }
            }
        }
    }

    private void OnQTEComplete()
    {
        QTEInProgress = false;
        IngredientObject ig = (IngredientObject)holdable;
        ig.ChangeState(IngredientState.Cooked);
    }

    protected override void OnZ()
    {
        Debug.Log("OnZ");
        if (!QTEInProgress)
        {
            Debug.Log("Not in QTE");
            if (PlayerController.HoldingItem())
            {
                Debug.Log("Player holding item");
                if (PlaceItem(PlayerController.GetItem()))
                {
                    // visual feedback for placing item?
                }
            }
            else
            {
                Debug.Log("Player not holding item");
                if (holdable != null && holdable is IngredientObject ig)
                {
                    Debug.Log("Holdable is ingredient object");
                    if (ig.State() == IngredientState.Cooked) TakeItem();
                    else QTEInProgress = true;
                }

            }
        }
    }
}