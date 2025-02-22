using UnityEngine;

public class VFXSortOrder : MonoBehaviour
{
    public int sortOrder = 2;
    [SerializeField] private Renderer vfxRenderer;

    void OnValidate()
    {
        vfxRenderer = GetComponent<Renderer>();
        if (vfxRenderer != null)
        {
            vfxRenderer.sortingOrder = sortOrder;
        }
    }
    //0.4
}
