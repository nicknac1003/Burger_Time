public class Trash : Interactable
{
    public override void InteractZ(bool held)
    {
        if(held == false) return;

        if(PlayerController.Instance.DestroyItem())
        {
            // visual effect or something?
        }
    }
}