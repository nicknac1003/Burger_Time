using UnityEngine;

public class CuttingBoard : Appliance
{
    [SerializeReference] private QuickTimeEvent useApplianceQTE = new MashQTE();
    private bool QTEInProgress = false;

    // private void Start() {
    //     useApplianceQTE = new MashQTE();
    // }


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
        if (!QTEInProgress)
        {
            if (PlayerController.HoldingItem())
            {
                Holdable playerHolding = PlayerController.GetItem();
                if (PlaceItem(playerHolding))
                {
                    // visual feedback for placing item?
                }
            }
            else
            {
                if (holdable != null && holdable is IngredientObject ig)
                {
                    if (ig.State() == IngredientState.Cooked) TakeItem();
                    else QTEInProgress = true;
                }

            }
        }
    }
}