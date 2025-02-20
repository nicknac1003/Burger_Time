public class Trash : Interactable
{
    protected override void OnZ()
    {
        if(PlayerController.Instance.DestroyItem())
        {
            // visual effect or something?
        }
    }
}