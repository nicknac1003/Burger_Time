public class Trash : Interactable
{
    protected override void OnZ()
    {
        if(PlayerController.DestroyItem())
        {
            // visual effect or something?
        }
    }
}