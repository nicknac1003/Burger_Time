using UnityEngine;

public class MashQTE : QuickTimeEvent
{
    [Tooltip("The number of times the player must press the button to succeed.")]
    [Range(3, 20)][SerializeField] private int mashCount = 10;

    [SerializeField] private GameObject fillBar;

    private GameObject fillBarInstance;
    private SpriteRenderer fillBarSpriteRenderer;

    public override void CreateUI(Transform parent)
    {
        fillBarInstance = Object.Instantiate(fillBar, parent);
        fillBarInstance.transform.localPosition = new Vector3(0f, 2f, 0f);

        fillBarSpriteRenderer = fillBarInstance.GetComponent<SpriteRenderer>();
        fillBarSpriteRenderer.material = new Material(fillBarSpriteRenderer.material);
        fillBarSpriteRenderer.material.SetColor("_colorEmpty", GlobalConstants.badColor);
        fillBarSpriteRenderer.material.SetColor("_colorFilled", GlobalConstants.goodColor);
    }

    public override void DestroyUI()
    {
        throw new System.NotImplementedException();
    }

    public override bool InProgress()
    {
        throw new System.NotImplementedException();
    }

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        throw new System.NotImplementedException();
    }
}