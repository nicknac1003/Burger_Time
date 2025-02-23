using UnityEngine;
using UnityEngine.UI;

public class RatingUI : MonoBehaviour
{
    public static RatingUI Instance { get; private set; }

    [SerializeField] private RectMask2D  mask;

    private float width;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        width = mask.rectTransform.sizeDelta.x;
    }

    public static void UpdateRating(float rating)
    {
        Instance.mask.padding = new Vector4(0, 0, Instance.width * (1 - rating), 0);
    }
}
