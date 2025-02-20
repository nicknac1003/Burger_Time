using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIBlink : MonoBehaviour
{
    [SerializeField] private float frequency = 1.0f;
    
    private float timer = 0;
    private CanvasGroup canvasGroup;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if(timer >= 1)
        {
            canvasGroup.alpha = 1 - canvasGroup.alpha;
            timer = 0;
        }
        
        timer += Time.deltaTime * frequency;
    }
}
